using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

public class ComicReportRepository : Repository<ComicReport>, IComicReportRepository
{
    public ComicReportRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<IEnumerable<ComicReport>> GetByStatusAsync(ReportStatus? status, int offset, int limit)
    {
        offset = Math.Max(offset, 0);
        limit = Math.Clamp(limit, 1, 100);
        var cacheKey = status.HasValue ? $"status:{status}:{offset}:{limit}" : $"all:{offset}:{limit}";

        return await _redisCache.GetFromRedisAsync<ComicReport>(
            () =>
            {
                var query = _dbSet.AsNoTracking().OrderByDescending(r => r.created_at);
                if (status.HasValue)
                {
                    query = query.Where(r => r.status == status.Value).OrderByDescending(r => r.created_at);
                }

                return query
                    .Skip(offset)
                    .Take(limit)
                    .ToListAsync();
            },
            cacheKey,
            DefaultCacheMinutes
        ) ?? [];
    }

    public async Task<IEnumerable<ComicReport>> GetByUserIdAsync(long userId, int offset, int limit)
    {
        offset = Math.Max(offset, 0);
        limit = Math.Clamp(limit, 1, 100);
        return await _redisCache.GetFromRedisAsync<ComicReport>(
            () => _dbSet.AsNoTracking()
                .Where(r => r.reporter_id == userId)
                .OrderByDescending(r => r.created_at)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(),
            $"user:{userId}:{offset}:{limit}",
            DefaultCacheMinutes
        ) ?? [];
    }

    public async Task<IEnumerable<ComicReport>> GetByComicOwnerAsync(long ownerId, int offset, int limit, ReportStatus? status = null)
    {
        offset = Math.Max(offset, 0);
        limit = Math.Clamp(limit, 1, 100);
        return await _redisCache.GetFromRedisAsync<ComicReport>(
            () => _dbSet.AsNoTracking()
                .Where(r => r.Comic != null && r.Comic.embedded_by == ownerId && (!status.HasValue || r.status == status.Value))
                .OrderByDescending(r => r.created_at)
                .ThenByDescending(r => r.id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(),
            $"owner:{ownerId}:s{status}:{offset}:{limit}",
            DefaultCacheMinutes
        ) ?? [];
    }
}
