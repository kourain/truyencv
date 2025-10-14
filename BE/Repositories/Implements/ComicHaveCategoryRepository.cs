using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicHaveCategory entity
/// </summary>
public class ComicHaveCategoryRepository : IComicHaveCategoryRepository
{
	private readonly AppDataContext _context;
	private readonly DbSet<ComicHaveCategory> _dbSet;
	private readonly IDistributedCache _redisCache;
	private const double DefaultCacheMinutes = 30;

	public ComicHaveCategoryRepository(AppDataContext context, IDistributedCache redisCache)
	{
		_context = context;
		_dbSet = context.Set<ComicHaveCategory>();
		_redisCache = redisCache;
	}

	public async Task<IEnumerable<ComicCategory>> GetCategoriesByComicIdAsync(long comicId)
	{
		return await _redisCache.GetFromRedisAsync<ComicCategory>(
			() => _dbSet.AsNoTracking()
				.Where(chc => chc.comic_id == comicId)
				.Select(chc => chc.ComicCategory!)
				.ToListAsync(),
			$"comic:{comicId}:categories",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetComicsByCategoryIdAsync(long categoryId)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking()
				.Where(chc => chc.comic_category_id == categoryId)
				.Select(chc => chc.Comic!)
				.ToListAsync(),
			$"category:{categoryId}:comics",
			DefaultCacheMinutes
		);
	}

	public async Task<ComicHaveCategory> AddAsync(long comicId, long categoryId)
	{
		var entity = new ComicHaveCategory
		{
			comic_id = comicId,
			comic_category_id = categoryId
		};

		await _dbSet.AddAsync(entity);
		await _context.SaveChangesAsync();

		// Xóa cache
		await _redisCache.RemoveAsync($"comic:{comicId}:categories");
		await _redisCache.RemoveAsync($"category:{categoryId}:comics");

		return entity;
	}

	public async Task<bool> DeleteAsync(long comicId, long categoryId)
	{
		var entity = await _dbSet.FirstOrDefaultAsync(chc =>
			chc.comic_id == comicId && chc.comic_category_id == categoryId);

		if (entity == null)
			return false;

		_dbSet.Remove(entity);
		await _context.SaveChangesAsync();

		// Xóa cache
		await _redisCache.RemoveAsync($"comic:{comicId}:categories");
		await _redisCache.RemoveAsync($"category:{categoryId}:comics");

		return true;
	}

	public async Task<bool> ExistsAsync(long comicId, long categoryId)
	{
		return await _dbSet.AnyAsync(chc =>
			chc.comic_id == comicId && chc.comic_category_id == categoryId);
	}
}
