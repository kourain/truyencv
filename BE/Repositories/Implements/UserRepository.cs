using System.Linq.Expressions;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho User entity
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _redisCache.GetFromRedisAsync<User>(
            _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.email == email),
            $"email:{email}",
			DefaultCacheMinutes
        );
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<User>(
            _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.id == id),
            $"{id}",
			DefaultCacheMinutes
		);
    }
}