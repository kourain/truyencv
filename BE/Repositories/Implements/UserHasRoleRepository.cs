using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho UserHasRole entity
/// </summary>
public class UserHasRoleRepository : Repository<UserHasRole>, IUserHasRoleRepository
{
    public UserHasRoleRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<IEnumerable<UserHasRole>> GetByUserIdAsync(long userId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasRole>(
            () => _dbSet.AsNoTracking().Where(r => r.user_id == userId).ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserHasRole>();
    }

    public async Task<IEnumerable<UserHasRole>> GetByRoleNameAsync(string roleName)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasRole>(
            () => _dbSet.AsNoTracking().Where(r => r.role_name == roleName).ToListAsync(),
            $"role:{roleName}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserHasRole>();
    }
}
