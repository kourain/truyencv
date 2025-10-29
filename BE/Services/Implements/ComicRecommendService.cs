using System.Linq;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

public class ComicRecommendService : IComicRecommendService
{
    private readonly IComicRecommendRepository _recommendRepository;
    private readonly IComicRepository _comicRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserUseCoinHistoryRepository _UserUseCoinHistoryRepository;
    private readonly IDistributedCache _redisCache;

    private const long RecommendCoinCost = 10;

    public ComicRecommendService(
        IComicRecommendRepository recommendRepository,
        IComicRepository comicRepository,
        IUserRepository userRepository,
        IUserUseCoinHistoryRepository UserUseCoinHistoryRepository,
        IDistributedCache redisCache)
    {
        _recommendRepository = recommendRepository;
        _comicRepository = comicRepository;
        _userRepository = userRepository;
        _UserUseCoinHistoryRepository = UserUseCoinHistoryRepository;
        _redisCache = redisCache;
    }

    public async Task<ComicRecommendResponse> CreateOrUpdateAsync(CreateComicRecommendRequest request)
    {
        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        await EnsureComicExists(comicId);

        if (request.rcm_count < 0)
        {
            throw new UserRequestException("Số lượt đề cử không hợp lệ");
        }

        var (month, year) = NormalizePeriod(request.month, request.year);

        var existing = await _recommendRepository.GetTrackedByComicAndPeriodAsync(comicId, month, year);
        if (existing != null)
        {
            existing.rcm_count = request.rcm_count;
            existing.month = month;
            existing.year = year;
            await _recommendRepository.UpdateAsync(existing);
            await _redisCache.AddOrUpdateInRedisAsync(existing, existing.id);
            return existing.ToRespDTO();
        }

        var recommend = request.ToEntity();
        recommend.comic_id = comicId;
        recommend.month = month;
        recommend.year = year;
        var created = await _recommendRepository.AddAsync(recommend);
        await _redisCache.AddOrUpdateInRedisAsync(created, created.id);
        return created.ToRespDTO();
    }

    public async Task<ComicRecommendResponse?> UpdateAsync(long id, UpdateComicRecommendRequest request)
    {
        var recommend = await _recommendRepository.GetByIdAsync(id);
        if (recommend == null)
        {
            return null;
        }

        if (request.rcm_count < 0)
        {
            throw new UserRequestException("Số lượt đề cử không hợp lệ");
        }

        if (request.month.HasValue || request.year.HasValue)
        {
            var (month, year) = NormalizePeriod(request.month ?? recommend.month, request.year ?? recommend.year);
            recommend.month = month;
            recommend.year = year;
        }
        recommend.rcm_count = request.rcm_count;

        await _recommendRepository.UpdateAsync(recommend);
        await _redisCache.AddOrUpdateInRedisAsync(recommend, recommend.id);
        return recommend.ToRespDTO();
    }

    public async Task<IEnumerable<ComicRecommendResponse>> GetTopAsync(int month, int year, int limit)
    {
        var (normalizedMonth, normalizedYear) = NormalizePeriod(month, year);
        var result = await _recommendRepository.GetTopAsync(normalizedMonth, normalizedYear, limit);
        return result.Select(r => r.ToRespDTO());
    }

    public async Task<IEnumerable<ComicRecommendResponse>> GetByComicAsync(long comicId, int limit)
    {
        var recommends = await _recommendRepository.GetByComicAsync(comicId, limit);
        return recommends.Select(r => r.ToRespDTO());
    }

    public async Task<ComicRecommendResponse?> GetByComicAndPeriodAsync(long comicId, int month, int year)
    {
        var (normalizedMonth, normalizedYear) = NormalizePeriod(month, year);
        var recommend = await _recommendRepository.GetByComicAndPeriodAsync(comicId, normalizedMonth, normalizedYear);
        return recommend?.ToRespDTO();
    }

    public async Task<ComicRecommendResponse> RecommendAsync(long comicId, long userId)
    {
        await EnsureComicExists(comicId);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserRequestException("Không tìm thấy người dùng");
        }

        if (user.coin < RecommendCoinCost)
        {
            throw new UserRequestException("Bạn không đủ xu để đề cử truyện này");
        }

        var (month, year) = NormalizePeriod(DateTime.UtcNow.Month, DateTime.UtcNow.Year);

        var existing = await _recommendRepository.GetTrackedByComicAndPeriodAsync(comicId, month, year);
        if (existing == null)
        {
            existing = new ComicRecommend
            {
                comic_id = comicId,
                month = month,
                year = year,
                rcm_count = 1,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };
            existing = await _recommendRepository.AddAsync(existing);
        }
        else
        {
            existing.rcm_count += 1;
            existing.updated_at = DateTime.UtcNow;
            await _recommendRepository.UpdateAsync(existing);
        }

        await _redisCache.AddOrUpdateInRedisAsync(existing, existing.id);

        user.coin -= RecommendCoinCost;
        await _userRepository.UpdateAsync(user);

        var historyRequest = new CreateUserUseCoinHistoryRequest
        {
            user_id = user.id.ToString(),
            coin = RecommendCoinCost,
            status = HistoryStatus.Use,
            reason = "Đề cử truyện",
            reference_id = comicId.ToString(),
            reference_type = "comic_recommend"
        };
        await _UserUseCoinHistoryRepository.AddAsync(historyRequest.ToEntity());

        return existing.ToRespDTO();
    }

    private async Task EnsureComicExists(long comicId)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            throw new UserRequestException("Comic không tồn tại");
        }
    }

    private static (int Month, int Year) NormalizePeriod(int month, int year)
    {
        var normalizedMonth = month is >= 1 and <= 12 ? month : DateTime.UtcNow.Month;
        var normalizedYear = year >= 2000 ? year : DateTime.UtcNow.Year;
        return (normalizedMonth, normalizedYear);
    }
}
