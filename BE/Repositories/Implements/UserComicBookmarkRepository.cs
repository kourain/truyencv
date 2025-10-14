using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho UserComicBookmark entity
/// </summary>
public class UserComicBookmarkRepository : Repository<UserComicBookmark>, IUserComicBookmarkRepository
{
    public UserComicBookmarkRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<UserComicBookmark?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<UserComicBookmark>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(b => b.id == id && b.deleted_at == null),
            id,
            DefaultCacheMinutes
        );
    }

    public async Task<UserComicBookmark?> GetByUserAndComicAsync(long userId, long comicId)
    {
        return await _redisCache.GetFromRedisAsync<UserComicBookmark>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(b => b.user_id == userId && b.comic_id == comicId && b.deleted_at == null),
            $"user:{userId}:comic:{comicId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<UserComicBookmark>> GetByUserIdAsync(long userId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserComicBookmark>(
            () => _dbSet.AsNoTracking()
                .Where(b => b.user_id == userId && b.deleted_at == null)
                .OrderByDescending(b => b.updated_at)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserComicBookmark>();
    }

    public async Task<IEnumerable<UserComicBookmark>> GetByComicIdAsync(long comicId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserComicBookmark>(
            () => _dbSet.AsNoTracking()
                .Where(b => b.comic_id == comicId && b.deleted_at == null)
                .ToListAsync(),
            $"comic:{comicId}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserComicBookmark>();
    }
}
