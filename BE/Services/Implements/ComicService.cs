using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Pgvector;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của Comic Service
/// </summary>
public class ComicService : IComicService
{
	private readonly IComicRepository _comicRepository;
	private readonly IDistributedCache _redisCache;
    private readonly ITextEmbeddingService _embeddingService;

	public ComicService(IComicRepository comicRepository, IDistributedCache redisCache, ITextEmbeddingService embeddingService)
	{
		_comicRepository = comicRepository;
		_redisCache = redisCache;
        _embeddingService = embeddingService;
	}

	public async Task<ComicResponse?> GetComicByIdAsync(long id)
	{
		var comic = await _comicRepository.GetByIdAsync(id);
		return comic?.ToRespDTO();
	}

	public async Task<ComicResponse?> GetComicBySlugAsync(string slug)
	{
		var comic = await _comicRepository.GetBySlugAsync(slug);
		return comic?.ToRespDTO();
	}

	public async Task<IEnumerable<ComicResponse>> SearchComicsAsync(string keyword, int limit, double minScore)
	{
		var normalizedKeyword = keyword?.Trim();
		if (string.IsNullOrWhiteSpace(normalizedKeyword))
			return [];

        limit = Math.Clamp(limit, 1, _embeddingService.Options.MaxResults);
        minScore = Math.Clamp(minScore, 0.0, 0.99);

		Vector? queryVector = null;
		if (_embeddingService.TryCreateEmbedding(out var embeddingValues, normalizedKeyword))
		{
			queryVector = new Vector(embeddingValues);
		}

		var comics = await _comicRepository.SearchAsync(queryVector, normalizedKeyword, limit, minScore);
		return comics.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicResponse>> GetComicsByAuthorAsync(string author)
	{
		var comics = await _comicRepository.GetByAuthorAsync(author);
		return comics.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicResponse>> GetComicsByStatusAsync(ComicStatus status)
	{
		var comics = await _comicRepository.GetByStatusAsync(status);
		return comics.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicResponse>> GetComicsAsync(int offset, int limit)
	{
		var comics = await _comicRepository.GetPagedAsync(offset, limit);
		return comics.Select(c => c.ToRespDTO());
	}

	public async Task<ComicResponse> CreateComicAsync(CreateComicRequest comicRequest)
	{
		// Kiểm tra slug đã tồn tại chưa
		if (await _comicRepository.ExistsAsync(c => c.slug == comicRequest.slug))
			throw new Exception("Slug đã tồn tại");

		// Chuyển đổi từ DTO sang Entity
		var comic = comicRequest.ToEntity();

		if (_embeddingService.TryCreateEmbedding(out var embeddingValues, comic.name, comic.description))
		{
			comic.search_vector = new Vector(embeddingValues);
		}

		// Thêm comic vào database
		var newComic = await _comicRepository.AddAsync(comic);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(newComic, newComic.id);

		return newComic.ToRespDTO();
	}

	public async Task<ComicResponse?> UpdateComicAsync(long id, UpdateComicRequest comicRequest)
	{
		// Lấy comic từ database
		var comic = await _comicRepository.GetByIdAsync(id);
		if (comic == null)
			return null;

		// Cập nhật thông tin
		comic.UpdateFromRequest(comicRequest);

		if (_embeddingService.TryCreateEmbedding(out var embeddingValues, comic.name, comic.description))
		{
			comic.search_vector = new Vector(embeddingValues);
		}
		else
		{
			comic.search_vector = null;
		}

		// Cập nhật vào database
		await _comicRepository.UpdateAsync(comic);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);

		return comic.ToRespDTO();
	}

	public async Task<bool> DeleteComicAsync(long id)
	{
		// Lấy comic từ database
		var comic = await _comicRepository.GetByIdAsync(id);
		if (comic == null)
			return false;

		// Soft delete: cập nhật deleted_at
		comic.deleted_at = DateTime.UtcNow;
		await _comicRepository.UpdateAsync(comic);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);

		return true;
	}
}
