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

	public async Task<IEnumerable<Comic>> SearchAsync(Vector? vector, string keyword, int limit, double minScore)
	{
		if (string.IsNullOrWhiteSpace(keyword))
			return [];

		limit = Math.Clamp(limit, 1, 50);
		minScore = Math.Clamp(minScore, 0.0, 0.99);
		var results = new List<Comic>(capacity: limit);

		if (vector is { } vectorQuery)
		{
			var vectorResults = await QueryByVectorAsync(vectorQuery, limit, minScore);
			results.AddRange(vectorResults);
		}

		if (results.Count < limit)
		{
			var remaining = limit - results.Count;
			var excluded = results.Count > 0 ? results.Select(c => c.id).ToArray() : Array.Empty<long>();
			var fallback = await SearchFallbackAsync(keyword, remaining, excluded);
			results.AddRange(fallback);
		}

		return results;
	}

	private async Task<List<Comic>> QueryByVectorAsync(Vector queryVector, int limit, double minScore)
	{
		var query = _dbSet.AsNoTracking()
			.Where(c => c.search_vector != null);

		if (minScore > 0)
		{
			var distanceThreshold = (float)(1.0 - minScore);
			query = query.Where(c => c.search_vector!.CosineDistance(queryVector) <= distanceThreshold);
		}

		var orderedQuery = query
			.OrderBy(c => c.search_vector!.CosineDistance(queryVector))
			.Take(limit);

		var candidates = await orderedQuery.ToListAsync();
		if (candidates.Count == 0 && minScore > 0)
		{
			candidates = await _dbSet.AsNoTracking()
				.Where(c => c.search_vector != null)
				.OrderBy(c => c.search_vector!.CosineDistance(queryVector))
				.Take(limit)
				.ToListAsync();
		}

		return candidates;
	}

	private Task<List<Comic>> SearchFallbackAsync(string keyword, int limit, long[]? excludedIds)
	{
		if (limit <= 0)
			return Task.FromResult(new List<Comic>());

		var sanitized = SanitizeKeyword(keyword);
		var pattern = $"%{sanitized}%";

		var query = _dbSet.AsNoTracking()
			.Where(c => EF.Functions.ILike(c.name, pattern, "\\")
				|| EF.Functions.ILike(c.description, pattern, "\\")
				|| EF.Functions.ILike(c.author, pattern, "\\"));

		if (excludedIds is { Length: > 0 })
		{
			query = query.Where(c => !excludedIds.Contains(c.id));
		}

		return query
			.OrderByDescending(c => c.updated_at)
			.ThenBy(c => c.id)
			.Take(limit)
			.ToListAsync();
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
