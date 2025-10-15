using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho Comic Service
/// </summary>
public interface IComicService
{
	/// <summary>
	/// Lấy thông tin comic theo ID
	/// </summary>
	/// <param name="id">ID của comic</param>
	/// <returns>Thông tin comic</returns>
	Task<ComicResponse?> GetComicByIdAsync(long id);

	/// <summary>
	/// Lấy thông tin comic theo slug
	/// </summary>
	/// <param name="slug">Slug của comic</param>
	/// <returns>Thông tin comic</returns>
	Task<ComicResponse?> GetComicBySlugAsync(string slug);

	/// <summary>
	/// Tìm kiếm comic
	/// </summary>
	/// <param name="keyword">Từ khóa tìm kiếm</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> SearchComicsAsync(string keyword);

	/// <summary>
	/// Lấy danh sách comic theo tác giả
	/// </summary>
	/// <param name="author">Tên tác giả</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsByAuthorAsync(string author);

	/// <summary>
	/// Lấy danh sách comic theo trạng thái
	/// </summary>
	/// <param name="status">Trạng thái comic</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsByStatusAsync(ComicStatus status);

	/// <summary>
	/// Lấy danh sách comic với phân trang
	/// </summary>
	/// <param name="offset">Vị trí bắt đầu</param>
	/// <param name="limit">Số lượng bản ghi</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsAsync(int offset, int limit);

	/// <summary>
	/// Tạo comic mới
	/// </summary>
	/// <param name="comicRequest">Thông tin comic mới</param>
	/// <returns>Thông tin comic đã tạo</returns>
	Task<ComicResponse> CreateComicAsync(CreateComicRequest comicRequest);

	/// <summary>
	/// Cập nhật thông tin comic
	/// </summary>
	/// <param name="id">ID của comic</param>
	/// <param name="comicRequest">Thông tin cập nhật</param>
	/// <returns>Thông tin comic đã cập nhật</returns>
	Task<ComicResponse?> UpdateComicAsync(long id, UpdateComicRequest comicRequest);

	/// <summary>
	/// Xóa comic
	/// </summary>
	/// <param name="id">ID của comic</param>
	/// <returns>True nếu xóa thành công, ngược lại là False</returns>
	Task<bool> DeleteComicAsync(long id);
}
