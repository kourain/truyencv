using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho ComicHaveCategory Service
/// </summary>
public interface IComicHaveCategoryService
{
	/// <summary>
	/// Lấy danh sách categories của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách category</returns>
	Task<IEnumerable<ComicCategoryResponse>> GetCategoriesByComicIdAsync(long comicId);

	/// <summary>
	/// Lấy danh sách comics của một category
	/// </summary>
	/// <param name="categoryId">ID của category</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsByCategoryIdAsync(long categoryId);

	/// <summary>
	/// Thêm comic vào category
	/// </summary>
	/// <param name="request">Thông tin comic và category</param>
	/// <returns>True nếu thêm thành công</returns>
	Task<bool> AddComicToCategoryAsync(CreateComicHaveCategoryRequest request);

	/// <summary>
	/// Xóa comic khỏi category
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>True nếu xóa thành công</returns>
	Task<bool> RemoveComicFromCategoryAsync(long comicId, long categoryId);
}
