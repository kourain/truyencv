using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của UserComicReadHistory Service
/// </summary>
public class UserComicReadHistoryService : IUserComicReadHistoryService
{
    private readonly IUserComicReadHistoryRepository _readHistoryRepository;
    private readonly IDistributedCache _redisCache;

    public UserComicReadHistoryService(
        IUserComicReadHistoryRepository readHistoryRepository,
        IDistributedCache redisCache)
    {
        _readHistoryRepository = readHistoryRepository;
        _redisCache = redisCache;
    }

    public async Task<IEnumerable<UserComicReadHistoryResponse>> GetReadHistoryByUserIdAsync(long userId, int limit = 20)
    {
        var histories = await _readHistoryRepository.GetByUserIdAsync(userId, limit);
        return histories.Select(h => h.ToRespDTO());
    }

    public async Task<UserComicReadHistoryResponse> UpsertReadHistoryAsync(long userId, long comicId, long chapterId)
    {
        var payload = new UpsertUserComicReadHistoryRequest
        {
            comic_id = comicId.ToString(),
            chapter_id = chapterId.ToString()
        };

        var existing = await _readHistoryRepository.GetByUserAndComicAsync(userId, comicId);
        if (existing != null)
        {
            existing.UpdateFromRequest(payload);
            await _readHistoryRepository.UpdateAsync(existing);
            await InvalidateCaches(userId, comicId);
            return existing.ToRespDTO();
        }

        var history = payload.ToEntity(userId);
        var newHistory = await _readHistoryRepository.AddAsync(history);
        await InvalidateCaches(userId, comicId);

        return newHistory.ToRespDTO();
    }

    public async Task<bool> RemoveReadHistoryAsync(long userId, long comicId)
    {
        var existing = await _readHistoryRepository.GetByUserAndComicAsync(userId, comicId);
        if (existing == null)
        {
            return false;
        }

        await _readHistoryRepository.DeleteAsync(existing);
        await InvalidateCaches(userId, comicId);
        return true;
    }

    private async Task InvalidateCaches(long userId, long comicId)
    {
        await _redisCache.RemoveAsync($"UserComicReadHistory:user:{userId}");
        await _redisCache.RemoveAsync($"UserComicReadHistory:one:user:{userId}:comic:{comicId}");
    }
}
