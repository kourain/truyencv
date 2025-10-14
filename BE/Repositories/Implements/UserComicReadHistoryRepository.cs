using System.Linq;
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

    public async Task<UserComicReadHistory?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<UserComicReadHistory>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(h => h.id == id && h.deleted_at == null),
            id,
            DefaultCacheMinutes
        );
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
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return (result ?? Enumerable.Empty<UserComicReadHistory>()).Take(safeLimit);
    }
}
