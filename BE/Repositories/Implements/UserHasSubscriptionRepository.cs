using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

public class UserHasSubscriptionRepository : Repository<UserHasSubscription>, IUserHasSubscriptionRepository
{
    public UserHasSubscriptionRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<IEnumerable<UserHasSubscription>> GetByUserIdAsync(long userId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasSubscription>(
            () => _dbSet.AsNoTracking()
                .Where(subscription => subscription.user_id == userId && subscription.deleted_at == null)
                .OrderByDescending(subscription => subscription.created_at)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? [];
    }
}
