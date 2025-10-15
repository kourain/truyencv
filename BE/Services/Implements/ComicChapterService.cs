using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của ComicChapter Service
/// </summary>
public class ComicChapterService : IComicChapterService
{
	private readonly IComicChapterRepository _chapterRepository;
	private readonly IComicRepository _comicRepository;
	private readonly IDistributedCache _redisCache;

	public ComicChapterService(
		IComicChapterRepository chapterRepository,
		IComicRepository comicRepository,
		IDistributedCache redisCache)
	{
		_chapterRepository = chapterRepository;
		_comicRepository = comicRepository;
		_redisCache = redisCache;
	}

	public async Task<ComicChapterResponse?> GetChapterByIdAsync(long id)
	{
		var chapter = await _chapterRepository.GetByIdAsync(id);
		return chapter?.ToRespDTO();
	}

	public async Task<IEnumerable<ComicChapterResponse>> GetChaptersByComicIdAsync(long comicId)
	{
		var chapters = await _chapterRepository.GetByComicIdAsync(comicId);
		return chapters.Select(c => c.ToRespDTO());
	}

	public async Task<ComicChapterResponse?> GetChapterByComicIdAndChapterAsync(long comicId, int chapter)
	{
		var chapterEntity = await _chapterRepository.GetByComicIdAndChapterAsync(comicId, chapter);
		return chapterEntity?.ToRespDTO();
	}

	public async Task<ComicChapterResponse> CreateChapterAsync(CreateComicChapterRequest chapterRequest)
	{
		var comicId = chapterRequest.comic_id.ToSnowflakeId(nameof(chapterRequest.comic_id));

		// Kiểm tra comic có tồn tại không
		var comic = await _comicRepository.GetByIdAsync(comicId);
		if (comic == null)
			throw new Exception("Comic không tồn tại");

		// Kiểm tra chapter đã tồn tại chưa
		if (await _chapterRepository.ExistsAsync(c =>
			c.comic_id == comicId && c.chapter == chapterRequest.chapter))
			throw new Exception("Chapter đã tồn tại");

		// Chuyển đổi từ DTO sang Entity
		var chapter = chapterRequest.ToEntity();

		// Thêm chapter vào database
		var newChapter = await _chapterRepository.AddAsync(chapter);

		// Cập nhật số lượng chapter của comic
		comic.chapter_count++;
		await _comicRepository.UpdateAsync(comic);
		await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(newChapter, newChapter.id);

		return newChapter.ToRespDTO();
	}

	public async Task<ComicChapterResponse?> UpdateChapterAsync(long id, UpdateComicChapterRequest chapterRequest)
	{
		// Lấy chapter từ database
		var chapter = await _chapterRepository.GetByIdAsync(id);
		if (chapter == null)
			return null;

		// Cập nhật thông tin
		chapter.UpdateFromRequest(chapterRequest);

		// Cập nhật vào database
		await _chapterRepository.UpdateAsync(chapter);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(chapter, chapter.id);

		return chapter.ToRespDTO();
	}

	public async Task<bool> DeleteChapterAsync(long id)
	{
		// Lấy chapter từ database
		var chapter = await _chapterRepository.GetByIdAsync(id);
		if (chapter == null)
			return false;

		// Lấy comic để cập nhật số lượng chapter
		var comic = await _comicRepository.GetByIdAsync(chapter.comic_id);
		if (comic != null)
		{
			comic.chapter_count = Math.Max(0, comic.chapter_count - 1);
			await _comicRepository.UpdateAsync(comic);
			await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);
		}

		// Soft delete: cập nhật deleted_at
		chapter.deleted_at = DateTime.UtcNow;
		await _chapterRepository.UpdateAsync(chapter);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(chapter, chapter.id);

		return true;
	}
}
