using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của ComicHaveCategory Service
/// </summary>
public class ComicHaveCategoryService : IComicHaveCategoryService
{
	private readonly IComicHaveCategoryRepository _comicHaveCategoryRepository;
	private readonly IComicRepository _comicRepository;
	private readonly IComicCategoryRepository _categoryRepository;
	private readonly IDistributedCache _redisCache;

	public ComicHaveCategoryService(
		IComicHaveCategoryRepository comicHaveCategoryRepository,
		IComicRepository comicRepository,
		IComicCategoryRepository categoryRepository,
		IDistributedCache redisCache)
	{
		_comicHaveCategoryRepository = comicHaveCategoryRepository;
		_comicRepository = comicRepository;
		_categoryRepository = categoryRepository;
		_redisCache = redisCache;
	}

	public async Task<IEnumerable<ComicCategoryResponse>> GetCategoriesByComicIdAsync(long comicId)
	{
		var categories = await _comicHaveCategoryRepository.GetCategoriesByComicIdAsync(comicId);
		return categories.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicResponse>> GetComicsByCategoryIdAsync(long categoryId)
	{
		var comics = await _comicHaveCategoryRepository.GetComicsByCategoryIdAsync(categoryId);
		return comics.Select(c => c.ToRespDTO());
	}

	public async Task<bool> AddComicToCategoryAsync(CreateComicHaveCategoryRequest request)
	{
		// Kiểm tra comic có tồn tại không
		var comic = await _comicRepository.GetByIdAsync(request.comic_id);
		if (comic == null)
			throw new Exception("Comic không tồn tại");

		// Kiểm tra category có tồn tại không
		var category = await _categoryRepository.GetByIdAsync(request.comic_category_id);
		if (category == null)
			throw new Exception("Category không tồn tại");

		// Kiểm tra đã tồn tại chưa
		if (await _comicHaveCategoryRepository.ExistsAsync(request.comic_id, request.comic_category_id))
			throw new Exception("Comic đã có trong category này");

		// Thêm vào database
		await _comicHaveCategoryRepository.AddAsync(request.comic_id, request.comic_category_id);

		return true;
	}

	public async Task<bool> RemoveComicFromCategoryAsync(long comicId, long categoryId)
	{
		// Xóa khỏi database
		return await _comicHaveCategoryRepository.DeleteAsync(comicId, categoryId);
	}
}
