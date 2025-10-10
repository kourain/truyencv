using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho ComicCategory entity
/// </summary>
public interface IComicCategoryRepository : IRepository<ComicCategory>
{
	/// <summary>
	/// Lấy category theo id
	/// </summary>
	/// <param name="id">ID của category</param>
	/// <returns>Category nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicCategory?> GetByIdAsync(ulong id);

	/// <summary>
	/// Lấy category theo tên
	/// </summary>
	/// <param name="name">Tên category</param>
	/// <returns>Category nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicCategory?> GetByNameAsync(string name);

	/// <summary>
	/// Lấy danh sách category mới nhất
	/// </summary>
	/// <param name="limit">Số lượng category cần lấy</param>
	/// <returns>Danh sách category theo thứ tự thời gian tạo</returns>
	Task<IEnumerable<ComicCategory>> GetLatestAsync(int limit);
}
