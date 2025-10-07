using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicChapter entity
/// </summary>
public class ComicChapterRepository : Repository<ComicChapter>, IComicChapterRepository
{
	public ComicChapterRepository(DataContext context, IDistributedCache redisCache) : base(context, redisCache)
	{
	}

	public async Task<ComicChapter?> GetByIdAsync(long id)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			_dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.id == id),
			$"{id}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<ComicChapter>> GetByComicIdAsync(long comicId)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			_dbSet.AsNoTracking()
				.Where(c => c.comic_id == comicId)
				.OrderBy(c => c.chapter)
				.ToListAsync(),
			$"comic:{comicId}",
			DefaultCacheMinutes
		);
	}

	public async Task<ComicChapter?> GetByComicIdAndChapterAsync(long comicId, int chapter)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			_dbSet.AsNoTracking()
				.FirstOrDefaultAsync(c => c.comic_id == comicId && c.chapter == chapter),
			$"comic:{comicId}:chapter:{chapter}",
			DefaultCacheMinutes
		);
	}
}
