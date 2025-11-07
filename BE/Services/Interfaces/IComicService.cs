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
	/// Lấy thông tin SEO cho comic theo slug
	/// </summary>
	/// <param name="slug">Slug của comic</param>
	/// <returns>Thông tin SEO hoặc null nếu không tìm thấy</returns>
	Task<ComicSeoResponse?> GetComicSEOBySlugAsync(string slug);

	/// <summary>
	/// Lấy chi tiết comic hiển thị cho người dùng theo slug
	/// </summary>
	/// <param name="slug">Slug của comic</param>
	/// <returns>Thông tin comic hiển thị hoặc null nếu không tìm thấy</returns>
	Task<ComicResponse?> GetComicDetailBySlugAsync(string slug);

	/// <summary>
	/// Tìm kiếm comic
	/// </summary>
	/// <param name="keyword">Từ khóa tìm kiếm</param>
	/// <param name="limit">Giới hạn số kết quả trả về</param>
	/// <param name="minScore">Ngưỡng điểm tương đồng (0-1)</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> SearchComicsAsync(string keyword, int limit = EmbeddingDefaults.MaxResults, double minScore = EmbeddingDefaults.MinScore);

	/// <summary>
	/// Lấy danh sách comic theo tác giả
	/// </summary>
	/// <param name="author">Tên tác giả</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsByAuthorAsync(string author);

	/// <summary>
	/// Lấy danh sách truyện được nhúng bởi một user (embedded_by)
	/// </summary>
	/// <param name="embeddedBy">ID user nhúng (long)</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<ComicResponse>> GetComicsByEmbeddedByAsync(long embeddedBy);

	/// <summary>
	/// Lấy danh sách truyện được nhúng bởi cùng user với truyện có slug
	/// </summary>
	Task<IEnumerable<ComicResponse>> GetComicsByEmbeddedBySlugAsync(string slug);

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
	Task<ComicResponse> CreateComicAsync(CreateComicRequest comicRequest, long embedded_by);

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

	/// <summary>
	/// Lấy dữ liệu tổng hợp cho trang chủ người dùng
	/// </summary>
	/// <param name="userId">ID người dùng</param>
	/// <returns>Dữ liệu trang chủ</returns>
	Task<UserHomeResponse> GetHomeForUserAsync(long userId);
}
