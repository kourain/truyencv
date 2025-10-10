using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho ComicHaveCategory entity
/// </summary>
public interface IComicHaveCategoryRepository
{
	/// <summary>
	/// Lấy danh sách categories của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách category</returns>
	Task<IEnumerable<ComicCategory>> GetCategoriesByComicIdAsync(ulong comicId);

	/// <summary>
	/// Lấy danh sách comics của một category
	/// </summary>
	/// <param name="categoryId">ID của category</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<Comic>> GetComicsByCategoryIdAsync(ulong categoryId);

	/// <summary>
	/// Thêm comic vào category
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>ComicHaveCategory đã thêm</returns>
	Task<ComicHaveCategory> AddAsync(ulong comicId, ulong categoryId);

	/// <summary>
	/// Xóa comic khỏi category
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>True nếu xóa thành công</returns>
	Task<bool> DeleteAsync(ulong comicId, ulong categoryId);

	/// <summary>
	/// Kiểm tra xem comic có thuộc category không
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>True nếu comic thuộc category</returns>
	Task<bool> ExistsAsync(ulong comicId, ulong categoryId);
}
