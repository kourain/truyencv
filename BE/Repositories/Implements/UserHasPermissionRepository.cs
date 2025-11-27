using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho UserHasPermission entity
/// </summary>
public class UserHasPermissionRepository : Repository<UserHasPermission>, IUserHasPermissionRepository
{
    public UserHasPermissionRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<IEnumerable<UserHasPermission>> GetByUserIdAsync(long userId)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasPermission>(
            () => _dbSet.AsNoTracking()
                .Where(p => p.user_id == userId && p.deleted_at == null)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? [];
    }

    public async Task<IEnumerable<UserHasPermission>> GetByPermissionAsync(Permissions permission)
    {
        var result = await _redisCache.GetFromRedisAsync<UserHasPermission>(
            () => _dbSet.AsNoTracking()
                .Where(p => p.permissions == permission && p.deleted_at == null)
                .ToListAsync(),
            $"permission:{(int)permission}",
            DefaultCacheMinutes
        );
        return result ?? [];
    }

    public async Task<UserHasPermission?> GetByUserPermissionAsync(long userId, Permissions permission)
    {
        return await _redisCache.GetFromRedisAsync<UserHasPermission>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.user_id == userId && p.permissions == permission && p.deleted_at == null),
            $"user:{userId}:permission:{(int)permission}",
            DefaultCacheMinutes
        );
    }
}
