using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của ComicCategory Service
/// </summary>
public class ComicCategoryService : IComicCategoryService
{
	private readonly IComicCategoryRepository _categoryRepository;
	private readonly IDistributedCache _redisCache;

	public ComicCategoryService(IComicCategoryRepository categoryRepository, IDistributedCache redisCache)
	{
		_categoryRepository = categoryRepository;
		_redisCache = redisCache;
	}

	public async Task<ComicCategoryResponse?> GetCategoryByIdAsync(long id)
	{
		var category = await _categoryRepository.GetByIdAsync(id);
		return category?.ToRespDTO();
	}

	public async Task<ComicCategoryResponse?> GetCategoryByNameAsync(string name)
	{
		var category = await _categoryRepository.GetByNameAsync(name);
		return category?.ToRespDTO();
	}

	public async Task<IEnumerable<ComicCategoryResponse>> GetCategoriesAsync(int offset, int limit)
	{
		var categories = await _categoryRepository.GetPagedAsync(offset, limit);
		return categories.Select(c => c.ToRespDTO());
	}

	public async Task<IEnumerable<ComicCategoryResponse>> GetAllCategoriesAsync()
	{
		var categories = await _categoryRepository.GetAllAsync();
		return categories.Select(c => c.ToRespDTO());
	}

	public async Task<ComicCategoryResponse> CreateCategoryAsync(CreateComicCategoryRequest categoryRequest)
	{
		// Kiểm tra tên đã tồn tại chưa
		if (await _categoryRepository.ExistsAsync(c => c.name == categoryRequest.name))
			throw new Exception("Tên category đã tồn tại");

		// Chuyển đổi từ DTO sang Entity
		var category = categoryRequest.ToEntity();

		// Thêm category vào database
		var newCategory = await _categoryRepository.AddAsync(category);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(newCategory, newCategory.id);

		return newCategory.ToRespDTO();
	}

	public async Task<ComicCategoryResponse?> UpdateCategoryAsync(long id, UpdateComicCategoryRequest categoryRequest)
	{
		// Lấy category từ database
		var category = await _categoryRepository.GetByIdAsync(id);
		if (category == null)
			return null;

		// Cập nhật thông tin
		category.UpdateFromRequest(categoryRequest);

		// Cập nhật vào database
		await _categoryRepository.UpdateAsync(category);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(category, category.id);

		return category.ToRespDTO();
	}

	public async Task<bool> DeleteCategoryAsync(long id)
	{
		// Lấy category từ database
		var category = await _categoryRepository.GetByIdAsync(id);
		if (category == null)
			return false;

		// Soft delete: cập nhật deleted_at
		category.deleted_at = DateTime.UtcNow;
		await _categoryRepository.UpdateAsync(category);

		// Cập nhật cache
		await _redisCache.AddOrUpdateInRedisAsync(category, category.id);

		return true;
	}
}
