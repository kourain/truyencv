using System;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;
using TruyenCV;
using Pgvector.EntityFrameworkCore;

namespace TruyenCV.Services;

public class UserComicUnlockHistoryService : IUserComicUnlockHistoryService
{
    private readonly IUserComicUnlockHistoryRepository _unlockRepository;
    private readonly IUserRepository _userRepository;
    private readonly IComicRepository _comicRepository;
    private readonly IComicChapterRepository _comicChapterRepository;
    private readonly IUserUseKeyHistoryRepository _userUseKeyHistoryRepository;
    private readonly IDistributedCache _redisCache;
    private readonly AppDataContext _dbcontext;
    public UserComicUnlockHistoryService(
        IUserComicUnlockHistoryRepository unlockRepository,
        IUserRepository userRepository,
        IComicRepository comicRepository,
        IComicChapterRepository comicChapterRepository,
        IUserUseKeyHistoryRepository userUseKeyHistoryRepository,
        IDistributedCache redisCache,
        AppDataContext dbcontext)
    {
        _unlockRepository = unlockRepository;
        _userRepository = userRepository;
        _comicRepository = comicRepository;
        _comicChapterRepository = comicChapterRepository;
        _userUseKeyHistoryRepository = userUseKeyHistoryRepository;
        _redisCache = redisCache;
        _dbcontext = dbcontext;
    }

    public async Task<IEnumerable<UserComicUnlockHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _unlockRepository.GetByUserIdAsync(userId);
        return histories.Select(history => history.ToRespDTO());
    }

    public async Task<IEnumerable<UserComicUnlockHistoryResponse>> GetByComicIdAsync(long comicId)
    {
        var histories = await _unlockRepository.GetByComicIdAsync(comicId);
        return histories.Select(history => history.ToRespDTO());
    }

    public async Task<UserComicUnlockHistoryResponse> CreateAsync(CreateUserComicUnlockHistoryRequest request)
    {
        var userId = request.user_id.ToSnowflakeId(nameof(request.user_id));
        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        var chapterId = request.comic_chapter_id.ToSnowflakeId(nameof(request.comic_chapter_id));

        await EnsureUserExists(userId);
        await EnsureComicExists(comicId);
        await EnsureChapterBelongsToComicAsync(chapterId, comicId);

        var existing = await _unlockRepository.GetByUserAndChapterAsync(userId, chapterId);
        if (existing != null)
        {
            return existing.ToRespDTO();
        }

        var entity = request.ToEntity();
        var created = await _unlockRepository.AddAsync(entity);
        await InvalidateCachesAsync(userId, comicId, chapterId);
        return created.ToRespDTO();
    }

    public async Task<UserComicUnlockHistoryResponse> UnlockChapterAsync(long userId, UnlockComicChapterRequest request)
    {
        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        var chapterId = request.comic_chapter_id.ToSnowflakeId(nameof(request.comic_chapter_id));
        var strategy = _dbcontext.Database.CreateExecutionStrategy();
        _dbcontext.ChangeTracker.Clear();
        return await strategy.ExecuteAsync<object?, UserComicUnlockHistoryResponse>(
            null,
            async (_, _, cancellationToken) =>
            {
                using (var transaction = await _dbcontext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var user = await _userRepository.GetByIdAsync(userId);
                        // await EnsureComicExists(comicId);
                        var chapter = await EnsureChapterBelongsToComicAsync(chapterId, comicId);

                        var existing = await _unlockRepository.GetByUserAndChapterAsync(userId, chapterId);
                        if (existing != null)
                        {
                            return existing.ToRespDTO();
                        }

                        var requireKey = chapter.key_require > 0 && (!chapter.key_require_until.HasValue || DateTime.UtcNow <= chapter.key_require_until.Value);
                        var keyUsed = 0;
                        if (requireKey)
                        {
                            keyUsed = chapter.key_require;
                            if (user.key < keyUsed)
                            {
                                throw new UserRequestException("Bạn không đủ chìa khóa để mở khóa chương này");
                            }

                            user.key -= keyUsed;
                            await _userRepository.UpdateAsync(user);

                            var keyHistoryRequest = new CreateUserUseKeyHistoryRequest
                            {
                                user_id = user.id.ToString(),
                                key = keyUsed,
                                status = HistoryStatus.Use,
                                chapter_id = chapter.id.ToString(),
                                note = $"Mở khóa chương {chapter.chapter}"
                            };
                            await _userUseKeyHistoryRepository.AddAsync(keyHistoryRequest.ToEntity());
                        }

                        var unlockRequest = new CreateUserComicUnlockHistoryRequest
                        {
                            user_id = userId.ToString(),
                            comic_id = comicId.ToString(),
                            comic_chapter_id = chapterId.ToString()
                        };

                        var created = await _unlockRepository.AddAsync(unlockRequest.ToEntity());
                        await InvalidateCachesAsync(userId, comicId, chapterId);
                        await transaction.CommitAsync(cancellationToken);
                        return created.ToRespDTO();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw new UserRequestException("Mở khóa chương thất bại, vui lòng thử lại sau" + e);
                    }
                }
            },
            null,
            default);
    }

    public async Task<bool> HasUnlockedChapterAsync(long userId, long chapterId)
    {
        var history = await _unlockRepository.GetByUserAndChapterAsync(userId, chapterId);
        return history != null;
    }

    private async Task<User> EnsureUserExists(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserRequestException("Người dùng không tồn tại");
        }

        return user;
    }

    private async Task EnsureComicExists(long comicId)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            throw new UserRequestException("Truyện không tồn tại");
        }
    }

    private async Task<ComicChapter> EnsureChapterBelongsToComicAsync(long chapterId, long comicId)
    {
        var chapter = await _comicChapterRepository.GetByIdAsync(chapterId);
        if (chapter == null)
        {
            throw new UserRequestException("Chương không tồn tại");
        }

        if (chapter.comic_id != comicId)
        {
            throw new UserRequestException("Chương không thuộc truyện được yêu cầu");
        }

        return chapter;
    }

    private async Task InvalidateCachesAsync(long userId, long comicId, long chapterId)
    {
        await _redisCache.RemoveFromRedisAsync<UserComicUnlockHistory>($"user:{userId}");
        await _redisCache.RemoveFromRedisAsync<UserComicUnlockHistory>($"comic:{comicId}");
        await _redisCache.RemoveFromRedisAsync<UserComicUnlockHistory>($"user:{userId}:chapter:{chapterId}");
    }
}
