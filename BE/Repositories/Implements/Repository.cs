using System.Linq.Expressions;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation cơ sở cho tất cả các repository
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    public virtual double DefaultCacheMinutes => 5; // Mặc định cache 5 phút
    protected readonly AppDataContext _dbcontext;
    protected readonly IDistributedCache _redisCache;
    protected readonly DbSet<T> _dbSet;
    public Repository(AppDataContext context, IDistributedCache redisCache)
    {
        _dbcontext = context;
        _redisCache = redisCache;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _redisCache.GetFromRedisAsync<T>(
            () => _dbSet.AsNoTracking().ToListAsync(),
            DefaultCacheMinutes
        ) ?? [];
    }
    public virtual async Task<T?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<T>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.id == id),
            id,
            DefaultCacheMinutes
        );
    }
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        // Đối với các tìm kiếm động, không sử dụng cache
        return await _dbSet.AsNoTracking().Where(expression).ToListAsync() ?? [];
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        // Đối với các tìm kiếm động, không sử dụng cache trừ khi có key cụ thể
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(expression);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int offset, int limit)
    {
        return await _redisCache.GetFromRedisAsync<T>(
            () => _dbSet.AsNoTracking().Skip(offset).Take(limit).ToListAsync(),
            offset, limit
        ) ?? [];
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbcontext.SaveChangesAsync();
        await _redisCache.AddOrUpdateInRedisAsync(entity, DefaultCacheMinutes);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _dbcontext.SaveChangesAsync();
        await _redisCache.AddOrUpdateInRedisAsync(entity, DefaultCacheMinutes);
    }

    public virtual async Task DeleteAsync(T entity, bool softDelete = true)
    {
        if (softDelete == false)
            _dbcontext.NotSoftDelete();
        _dbSet.Remove(entity);
        await _dbcontext.SaveChangesAsync();
        await _redisCache.RemoveFromRedisAsync<T>(entity.id);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.CountAsync(expression);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AsNoTracking().AnyAsync(expression);
    }
}