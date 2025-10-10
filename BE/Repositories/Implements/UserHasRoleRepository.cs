using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho UserHasRole entity
/// </summary>
public class UserHasRoleRepository : Repository<UserHasRole>, IUserHasRoleRepository
{
    public UserHasRoleRepository(DataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<UserHasRole?> GetByIdAsync(ulong id)
    {
        return await _redisCache.GetFromRedisAsync<UserHasRole>(
            _dbSet.AsNoTracking().FirstOrDefaultAsync(r => r.id == id),
            $"{id}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<UserHasRole>> GetByUserIdAsync(ulong userId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasRole>(
            _dbSet.AsNoTracking().Where(r => r.user_id == userId).ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserHasRole>();
    }

    public async Task<IEnumerable<UserHasRole>> GetByRoleNameAsync(string roleName)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasRole>(
            _dbSet.AsNoTracking().Where(r => r.role_name == roleName).ToListAsync(),
            $"role:{roleName}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<UserHasRole>();
    }
}
