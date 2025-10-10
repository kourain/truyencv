using System;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicCategory entity
/// </summary>
public class ComicCategoryRepository : Repository<ComicCategory>, IComicCategoryRepository
{
	public ComicCategoryRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
	{
	}

	public async Task<ComicCategory?> GetByIdAsync(ulong id)
	{
		return await _redisCache.GetFromRedisAsync<ComicCategory>(
			_dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.id == id),
			$"{id}",
			DefaultCacheMinutes
		);
	}

	public async Task<ComicCategory?> GetByNameAsync(string name)
	{
		return await _redisCache.GetFromRedisAsync<ComicCategory>(
			_dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.name == name),
			$"name:{name}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<ComicCategory>> GetLatestAsync(int limit)
	{
		limit = Math.Max(1, Math.Min(limit, 50));
		return await _redisCache.GetFromRedisAsync<ComicCategory>(
			_dbSet.AsNoTracking()
				.OrderByDescending(c => c.created_at)
				.ThenByDescending(c => c.updated_at)
				.Take(limit)
				.ToListAsync(),
			$"latest:{limit}",
			DefaultCacheMinutes
		);
	}
}
