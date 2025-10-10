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

    public async Task<IEnumerable<UserComicReadHistoryResponse>> GetReadHistoryByUserIdAsync(ulong userId, int limit = 20)
    {
        var histories = await _readHistoryRepository.GetByUserIdAsync(userId, limit);
        return histories.Select(h => h.ToRespDTO());
    }

    public async Task<UserComicReadHistoryResponse> UpsertReadHistoryAsync(ulong userId, ulong comicId, ulong chapterId)
    {
        var payload = new UpsertUserComicReadHistoryRequest
        {
            comic_id = comicId,
            chapter_id = chapterId
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

    public async Task<bool> RemoveReadHistoryAsync(ulong userId, ulong comicId)
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

    private async Task InvalidateCaches(ulong userId, ulong comicId)
    {
        await _redisCache.RemoveAsync($"UserComicReadHistory:user:{userId}");
        await _redisCache.RemoveAsync($"UserComicReadHistory:one:user:{userId}:comic:{comicId}");
    }
}
