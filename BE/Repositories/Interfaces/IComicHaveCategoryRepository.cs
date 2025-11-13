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
	Task<IEnumerable<ComicCategory>> GetCategoriesByComicIdAsync(long comicId);

	/// <summary>
	/// Lấy danh sách comics của một category
	/// </summary>
	/// <param name="categoryId">ID của category</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<Comic>> GetComicsByCategoryIdAsync(long categoryId);

	/// <summary>
	/// Thêm comic vào category
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>ComicHaveCategory đã thêm</returns>
	Task<ComicHaveCategory> AddAsync(long comicId, long categoryId);

	/// <summary>
	/// Xóa comic khỏi category
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="categoryId">ID của category</param>
	/// <returns>True nếu xóa thành công</returns>
	Task<bool> DeleteAsync(long comicId, long categoryId);
    Task<int> UpdateAllOfComicAsync(long comicId, IEnumerable<long> newCategoryIds);
    Task<int> DeleteAllOfComicAsync(long comicId);

    /// <summary>
    /// Kiểm tra xem comic có thuộc category không
    /// </summary>
    /// <param name="comicId">ID của comic</param>
    /// <param name="categoryId">ID của category</param>
    /// <returns>True nếu comic thuộc category</returns>
    Task<bool> ExistsAsync(long comicId, long categoryId);
}
