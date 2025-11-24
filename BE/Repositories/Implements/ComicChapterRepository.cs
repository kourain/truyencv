using System;
using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicChapter entity
/// </summary>
public class ComicChapterRepository : Repository<ComicChapter>, IComicChapterRepository
{
	public ComicChapterRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
	{
	}

	public async Task<IEnumerable<ComicChapter>> GetByComicIdAsync(long comicId)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			() => _dbSet.AsNoTracking()
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
			() => _dbSet.AsNoTracking()
				.FirstOrDefaultAsync(c => c.comic_id == comicId && c.chapter == chapter),
			$"comic:{comicId}:chapter:{chapter}",
			DefaultCacheMinutes
		);
	}

	public async Task<ComicChapter?> GetPreviousChapterAsync(long comicId, int chapter)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			() => _dbSet.AsNoTracking()
				.Where(c => c.comic_id == comicId && c.chapter < chapter)
				.OrderByDescending(c => c.chapter)
				.FirstOrDefaultAsync(),
			$"comic:{comicId}:chapter:{chapter}:prev",
			DefaultCacheMinutes
		);
	}

	public async Task<ComicChapter?> GetNextChapterAsync(long comicId, int chapter)
	{
		return await _redisCache.GetFromRedisAsync<ComicChapter>(
			() => _dbSet.AsNoTracking()
				.Where(c => c.comic_id == comicId && c.chapter > chapter)
				.OrderBy(c => c.chapter)
				.FirstOrDefaultAsync(),
			$"comic:{comicId}:chapter:{chapter}:next",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<ComicChapter>> GetLatestUpdatedAsync(int limit)
	{
		limit = Math.Clamp(limit, 1, 50);

		return await _dbSet.AsNoTracking()
			.Include(chapter => chapter.Comic)
			.Where(chapter => chapter.deleted_at == null && chapter.Comic != null && chapter.Comic.deleted_at == null)
			.OrderByDescending(chapter => chapter.updated_at)
			.ThenByDescending(chapter => chapter.id)
			.Take(limit)
			.ToListAsync();
	}
}
