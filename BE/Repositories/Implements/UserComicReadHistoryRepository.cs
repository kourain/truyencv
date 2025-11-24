using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho UserComicReadHistory entity
/// </summary>
public class UserComicReadHistoryRepository : Repository<UserComicReadHistory>, IUserComicReadHistoryRepository
{
    public UserComicReadHistoryRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<UserComicReadHistory?> GetByUserAndComicAsync(long userId, long comicId)
    {
        return await _redisCache.GetFromRedisAsync<UserComicReadHistory>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(h => h.user_id == userId && h.comic_id == comicId && h.deleted_at == null),
            $"user:{userId}:comic:{comicId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<UserComicReadHistory>> GetByUserIdAsync(long userId, int limit)
    {
        var safeLimit = Math.Max(1, limit);
        var result = await _redisCache.GetFromRedisAsync<UserComicReadHistory>(
            () => _dbSet.AsNoTracking()
                .Where(h => h.user_id == userId && h.deleted_at == null)
                .OrderByDescending(h => h.updated_at)
                .Take(safeLimit)
                .Include(m => m.Comic)
                .Where(m => m.Comic != null && m.Comic.deleted_at == null && m.Comic.status != ComicStatus.Banned)
                .Include(m => m.ComicChapter)
                .Where(m => m.ComicChapter != null && m.ComicChapter.deleted_at == null)
                .ToListAsync(),
            $"user:{userId}:lm{safeLimit}",
            DefaultCacheMinutes
        );
        return (result ?? Enumerable.Empty<UserComicReadHistory>()).Take(safeLimit);
    }

    public async Task<IEnumerable<UserComicReadAggregate>> GetTopByUpdatedAtAsync(DateTime fromUtc, int limit)
    {
        limit = Math.Clamp(limit, 1, 50);

        var query = (await _redisCache.GetFromRedisAsync(
                () => _dbSet.AsNoTracking()
                .Include(history => history.Comic)
                .Where(history => history.deleted_at == null && history.updated_at >= fromUtc && history.Comic != null && history.Comic.deleted_at == null && history.Comic.status != ComicStatus.Banned)
                .OrderByDescending(history => history.updated_at)
                .Take(limit)
                .ToListAsync(), $"GetTopByUpdatedAtAsync:{fromUtc.ToString()}-{limit}", DefaultCacheMinutes)).GroupBy(history => history.comic_id);
        return query
            .Select(group => new UserComicReadAggregate(
                group.Key,
                group.LongCount(),
                group.Max(item => item.updated_at),
                group.FirstOrDefault()?.Comic.ToRespDTO()))
            .OrderByDescending(result => result.reader_count)
            .ThenByDescending(result => result.last_read_at);
    }

    public async Task<IDictionary<long, long>> GetReaderCountsAsync(IEnumerable<long> comicIds, int month = 3)
    {
        var ids = comicIds?.Distinct().ToArray() ?? Array.Empty<long>();
        if (ids.Length == 0)
        {
            return new Dictionary<long, long>();
        }

        var aggregates = await _dbSet.AsNoTracking()
            .Where(history => history.deleted_at == null && ids.Contains(history.comic_id) && history.created_at.Month >= DateTime.UtcNow.AddMonths(-month).Month)
            .GroupBy(history => history.comic_id)
            .Select(group => new
            {
                comic_id = group.Key,
                reader_count = group.LongCount()
            })
            .ToListAsync();

        return aggregates.ToDictionary(item => item.comic_id, item => item.reader_count);
    }
}
