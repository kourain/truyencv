using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của UserComicReadHistory Service
/// </summary>
public class UserComicReadHistoryService : IUserComicReadHistoryService
{
    private readonly IUserComicReadHistoryRepository _readHistoryRepository;
    private readonly IDistributedCache _redisCache;
    private readonly AppDataContext _dbcontext;

    public UserComicReadHistoryService(
        IUserComicReadHistoryRepository readHistoryRepository,
        IDistributedCache redisCache,
        AppDataContext dbcontext)
    {
        _readHistoryRepository = readHistoryRepository;
        _redisCache = redisCache;
        _dbcontext = dbcontext;
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
        var isNewComic = existing == null;
        var shouldIncreaseChapterCount = false;
        UserComicReadHistory targetHistory;

        if (existing != null)
        {
            shouldIncreaseChapterCount = existing.chapter_id != chapterId;
            existing.UpdateFromRequest(payload);
            await _readHistoryRepository.UpdateAsync(existing);
            targetHistory = existing;
        }
        else
        {
            var history = payload.ToEntity(userId);
            targetHistory = await _readHistoryRepository.AddAsync(history);
            shouldIncreaseChapterCount = true;
        }

        await InvalidateCaches(userId, comicId);

        if (isNewComic)
        {
            await IncrementReadComicCountAsync(userId);
        }

        if (shouldIncreaseChapterCount)
        {
            await IncrementReadChapterCountAsync(userId);
        }

        return targetHistory.ToRespDTO();
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

    private async Task IncrementReadComicCountAsync(long userId)
    {
        var utcNow = DateTime.UtcNow;
        var affectedRows = await _dbcontext.Users
            .Where(user => user.id == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(user => user.read_comic_count, user => user.read_comic_count + 1)
                .SetProperty(user => user.updated_at, _ => utcNow));

        if (affectedRows > 0)
        {
            await _redisCache.RemoveFromRedisAsync<User>(userId);
        }
    }

    private async Task IncrementReadChapterCountAsync(long userId)
    {
        var utcNow = DateTime.UtcNow;
        var affectedRows = await _dbcontext.Users
            .Where(user => user.id == userId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(user => user.read_chapter_count, user => user.read_chapter_count + 1)
                .SetProperty(user => user.updated_at, _ => utcNow));

        if (affectedRows > 0)
        {
            await _redisCache.RemoveFromRedisAsync<User>(userId);
        }
    }
}
