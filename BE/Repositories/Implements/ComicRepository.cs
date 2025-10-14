using System;
using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho Comic entity
/// </summary>
public class ComicRepository : Repository<Comic>, IComicRepository
{
	public ComicRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
	{
	}

	public async Task<Comic?> GetByIdAsync(long id)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.id == id),
			$"{id}",
			DefaultCacheMinutes
		);
	}

	public async Task<Comic?> GetBySlugAsync(string slug)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.slug == slug),
			$"slug:{slug}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> SearchAsync(string keyword)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking()
				.Where(c => c.name.Contains(keyword) || c.author.Contains(keyword) || c.description.Contains(keyword))
				.ToListAsync(),
			$"search:{keyword}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetByAuthorAsync(string author)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking()
				.Where(c => c.author == author)
				.ToListAsync(),
			$"author:{author}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetByStatusAsync(ComicStatus status)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking()
				.Where(c => c.status == status)
				.ToListAsync(),
			$"status:{status}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetTopRatedAsync(int limit)
	{
		limit = Math.Clamp(limit, 1, 50);
		return await _redisCache.GetFromRedisAsync<Comic>(
			_dbSet.AsNoTracking()
				.OrderByDescending(c => c.rate)
				.ThenByDescending(c => c.updated_at)
				.Take(limit)
				.ToListAsync(),
			$"top-rated:{limit}",
			DefaultCacheMinutes
		);
	}

	public async Task<long> SumBookmarkCountAsync()
	{
		return await _dbSet.AsNoTracking()
			.SumAsync(c => (long)c.bookmark_count);
	}
}
