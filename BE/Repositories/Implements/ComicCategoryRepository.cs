using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicCategory entity
/// </summary>
public class ComicCategoryRepository : Repository<ComicCategory>, IComicCategoryRepository
{
	public ComicCategoryRepository(DataContext context, IDistributedCache redisCache) : base(context, redisCache)
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
}
