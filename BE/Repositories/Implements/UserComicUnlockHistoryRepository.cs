using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

public class UserComicUnlockHistoryRepository : Repository<UserComicUnlockHistory>, IUserComicUnlockHistoryRepository
{
    public UserComicUnlockHistoryRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<UserComicUnlockHistory?> GetByUserAndChapterAsync(long userId, long chapterId)
    {
        return await _redisCache.GetFromRedisAsync<UserComicUnlockHistory>(
            () => _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(history => history.user_id == userId && history.comic_chapter_id == chapterId && history.deleted_at == null),
            $"user:{userId}:chapter:{chapterId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<UserComicUnlockHistory>> GetByUserIdAsync(long userId)
    {
        var histories = await _redisCache.GetFromRedisAsync<UserComicUnlockHistory>(
            () => _dbSet.AsNoTracking()
                .Where(history => history.user_id == userId && history.deleted_at == null)
                .OrderByDescending(history => history.created_at)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return histories ?? Enumerable.Empty<UserComicUnlockHistory>();
    }

    public async Task<IEnumerable<UserComicUnlockHistory>> GetByComicIdAsync(long comicId)
    {
        var histories = await _redisCache.GetFromRedisAsync<UserComicUnlockHistory>(
            () => _dbSet.AsNoTracking()
                .Where(history => history.comic_id == comicId && history.deleted_at == null)
                .OrderByDescending(history => history.created_at)
                .ToListAsync(),
            $"comic:{comicId}",
            DefaultCacheMinutes
        );
        return histories ?? Enumerable.Empty<UserComicUnlockHistory>();
    }
}
