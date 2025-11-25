using System;
using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho Comic entity
/// </summary>
public class ComicRepository : Repository<Comic>, IComicRepository
{
	public ComicRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
	{
	}

	public async Task<Comic?> GetBySlugAsync(string slug)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking().AsSplitQuery()
				.Include(c => c.ComicHaveCategories!)
				.ThenInclude(chc => chc.ComicCategory)
				.FirstOrDefaultAsync(c => c.slug == slug),
			$"slug:{slug}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<ComicSearchResult>> SearchAsync(Vector? vector, string keyword, int limit, double minScore)
	{
		if (string.IsNullOrWhiteSpace(keyword))
			return [];

		limit = Math.Clamp(limit, 1, 50);
		minScore = Math.Clamp(minScore, 0.0, 0.99);
		var results = new List<ComicSearchResult>(capacity: limit);

		if (vector is { } vectorQuery)
		{
			var vectorResults = await QueryByVectorAsync(vectorQuery, limit, minScore);
			results.AddRange(vectorResults);
		}

		if (results.Count < limit)
		{
			var remaining = limit - results.Count;
			var excluded = results.Count > 0 ? results.Select(r => r.Comic.id).ToArray() : Array.Empty<long>();
			var fallback = await SearchFallbackAsync(keyword, remaining, excluded);
			results.AddRange(fallback);
		}

		return results;
	}

	private async Task<List<ComicSearchResult>> QueryByVectorAsync(Vector queryVector, int limit, double minScore)
	{
		var query = _dbSet.AsNoTracking()
			.Where(c => c.search_vector != null && c.deleted_at == null);

		if (minScore > 0)
		{
			var distanceThreshold = (float)(1.0 - minScore);
			query = query.Where(c => c.search_vector!.CosineDistance(queryVector) <= distanceThreshold);
		}

		// Select with score calculation
		var results = await query
			.Select(c => new ComicSearchResult
			{
				Comic = c,
				Score = 1.0 - (double)c.search_vector!.CosineDistance(queryVector)
			})
			.OrderByDescending(r => r.Score)
			.Take(limit)
			.ToListAsync();

		// Fallback: nếu không có kết quả và có minScore, thử lại không filter score
		if (results.Count == 0 && minScore > 0)
		{
			results = await _dbSet.AsNoTracking()
				.Where(c => c.search_vector != null && c.deleted_at == null)
				.Select(c => new ComicSearchResult
				{
					Comic = c,
					Score = 1.0 - (double)c.search_vector!.CosineDistance(queryVector)
				})
				.OrderByDescending(r => r.Score)
				.Take(limit)
				.ToListAsync();
		}

		return results;
	}

	private async Task<List<ComicSearchResult>> SearchFallbackAsync(string keyword, int limit, long[]? excludedIds)
	{
		if (limit <= 0)
			return new List<ComicSearchResult>();

		var sanitized = SanitizeKeyword(keyword);
		var pattern = $"%{sanitized}%";

		var query = _dbSet.AsNoTracking()
			.Where(c => c.deleted_at == null
				&& (EF.Functions.ILike(c.name, pattern, "\\")
				|| EF.Functions.ILike(c.description, pattern, "\\")
				|| EF.Functions.ILike(c.author, pattern, "\\")));

		if (excludedIds is { Length: > 0 })
		{
			query = query.Where(c => !excludedIds.Contains(c.id));
		}

		var comics = await query
			.OrderByDescending(c => c.updated_at)
			.ThenBy(c => c.id)
			.Take(limit)
			.ToListAsync();

		// Text search fallback có score mặc định là 0.5
		return comics.Select(c => new ComicSearchResult
		{
			Comic = c,
			Score = 0.5 // Default score for text-based matches
		}).ToList();
	}

	private static string SanitizeKeyword(string keyword)
	{
		return keyword
			.Trim()
			.Replace("\\", "\\\\")
			.Replace("%", "\\%")
			.Replace("_", "\\_");
	}

	public async Task<IEnumerable<Comic>> GetByAuthorAsync(string author)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking()
				.Where(c => c.author == author)
				.ToListAsync(),
			$"author:{author}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetByEmbeddedByAsync(long embeddedBy, int offset = 0, int limit = 50)
	{
		offset = Math.Max(offset, 0);
		limit = Math.Clamp(limit, 1, 100);
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking()
				.Where(c => c.embedded_by == embeddedBy && c.deleted_at == null)
				.OrderByDescending(c => c.updated_at)
				.ThenByDescending(c => c.id)
				.Skip(offset)
				.Take(limit)
				.ToListAsync(),
			$"embedded_by:{embeddedBy}:{offset}:{limit}",
			DefaultCacheMinutes
		) ?? [];
	}

	public async Task<Comic?> GetByIdAndEmbeddedByAsync(long id, long embeddedBy)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking()
				.FirstOrDefaultAsync(c => c.id == id && c.embedded_by == embeddedBy && c.deleted_at == null),
			$"embedded_by:{embeddedBy}:comic:{id}",
			DefaultCacheMinutes
		);
	}

	public async Task<IEnumerable<Comic>> GetByStatusAsync(ComicStatus status)
	{
		return await _redisCache.GetFromRedisAsync<Comic>(
			() => _dbSet.AsNoTracking()
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
			() => _dbSet.AsNoTracking()
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
