using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

public class ComicRecommendRepository : Repository<ComicRecommend>, IComicRecommendRepository
{
    public ComicRecommendRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<ComicRecommend?> GetByComicAndPeriodAsync(long comicId, int month, int year)
    {
        return await _redisCache.GetFromRedisAsync<ComicRecommend>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.comic_id == comicId && r.month == month && r.year == year),
            $"comic:{comicId}:period:{year}-{month}",
            DefaultCacheMinutes
        );
    }

    public async Task<ComicRecommend?> GetTrackedByComicAndPeriodAsync(long comicId, int month, int year)
    {
        return await _dbSet.FirstOrDefaultAsync(r => r.comic_id == comicId && r.month == month && r.year == year);
    }

    public async Task<IEnumerable<ComicRecommend>> GetTopAsync(int month, int year, int limit)
    {
        limit = Math.Clamp(limit, 1, 50);
        return await _redisCache.GetFromRedisAsync<ComicRecommend>(
            () => _dbSet.AsNoTracking()
                .Where(r => r.month == month && r.year == year)
                .OrderByDescending(r => r.rcm_count)
                .ThenBy(r => r.comic_id)
                .Take(limit)
                .ToListAsync(),
            $"top:{year}-{month}:{limit}",
            DefaultCacheMinutes
        ) ?? [];
    }

    public async Task<IEnumerable<ComicRecommend>> GetByComicAsync(long comicId, int limit)
    {
        limit = Math.Clamp(limit, 1, 24);
        return await _redisCache.GetFromRedisAsync<ComicRecommend>(
            () => _dbSet.AsNoTracking()
                .Where(r => r.comic_id == comicId)
                .OrderByDescending(r => r.year)
                .ThenByDescending(r => r.month)
                .ThenByDescending(r => r.rcm_count)
                .Take(limit)
                .ToListAsync(),
            $"comic:{comicId}:recent:{limit}",
            DefaultCacheMinutes
        ) ?? [];
    }
}
