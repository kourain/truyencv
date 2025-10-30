using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Repositories;

public class UserComicRecommendRepository : Repository<UserComicRecommend>, IUserComicRecommendRepository
{
    public UserComicRecommendRepository(AppDataContext context, Microsoft.Extensions.Caching.Distributed.IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<UserComicRecommend?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<UserComicRecommend>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.id == id),
            id,
            DefaultCacheMinutes
        );
    }

    public async Task<UserComicRecommend?> GetByUserAndPeriodAsync(long userId, int month, int year)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.user_id == userId && r.month == month && r.year == year);
    }
}
