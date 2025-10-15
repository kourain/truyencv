using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của Comic Service
/// </summary>
public class ComicService : IComicService
{
	private readonly IComicRepository _comicRepository;
	private readonly IDistributedCache _redisCache;

	public ComicService(IComicRepository comicRepository, IDistributedCache redisCache)
	{
		_comicRepository = comicRepository;
		_redisCache = redisCache;
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

	public async Task<IEnumerable<ComicResponse>> SearchComicsAsync(string keyword)
	{
		var comics = await _comicRepository.SearchAsync(keyword);
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
