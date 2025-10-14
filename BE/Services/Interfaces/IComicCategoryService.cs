using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho ComicCategory Service
/// </summary>
public interface IComicCategoryService
{
	/// <summary>
	/// Lấy thông tin category theo ID
	/// </summary>
	/// <param name="id">ID của category</param>
	/// <returns>Thông tin category</returns>
	Task<ComicCategoryResponse?> GetCategoryByIdAsync(long id);

	/// <summary>
	/// Lấy thông tin category theo tên
	/// </summary>
	/// <param name="name">Tên category</param>
	/// <returns>Thông tin category</returns>
	Task<ComicCategoryResponse?> GetCategoryByNameAsync(string name);

	/// <summary>
	/// Lấy danh sách category với phân trang
	/// </summary>
	/// <param name="offset">Vị trí bắt đầu</param>
	/// <param name="limit">Số lượng bản ghi</param>
	/// <returns>Danh sách category</returns>
	Task<IEnumerable<ComicCategoryResponse>> GetCategoriesAsync(int offset, int limit);

	/// <summary>
	/// Lấy tất cả categories
	/// </summary>
	/// <returns>Danh sách tất cả category</returns>
	Task<IEnumerable<ComicCategoryResponse>> GetAllCategoriesAsync();

	/// <summary>
	/// Tạo category mới
	/// </summary>
	/// <param name="categoryRequest">Thông tin category mới</param>
	/// <returns>Thông tin category đã tạo</returns>
	Task<ComicCategoryResponse> CreateCategoryAsync(CreateComicCategoryRequest categoryRequest);

	/// <summary>
	/// Cập nhật thông tin category
	/// </summary>
	/// <param name="id">ID của category</param>
	/// <param name="categoryRequest">Thông tin cập nhật</param>
	/// <returns>Thông tin category đã cập nhật</returns>
	Task<ComicCategoryResponse?> UpdateCategoryAsync(long id, UpdateComicCategoryRequest categoryRequest);

	/// <summary>
	/// Xóa category
	/// </summary>
	/// <param name="id">ID của category</param>
	/// <returns>True nếu xóa thành công, ngược lại là False</returns>
	Task<bool> DeleteCategoryAsync(long id);
}
