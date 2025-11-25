using Pgvector;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho Comic entity
/// </summary>
public interface IComicRepository : IRepository<Comic>
{

	/// <summary>
	/// Lấy comic theo slug
	/// </summary>
	/// <param name="slug">Slug của comic</param>
	/// <returns>Comic nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<Comic?> GetBySlugAsync(string slug);

	/// <summary>
	/// Lấy danh sách comic do một người dùng nhúng
	/// </summary>
	/// <param name="embeddedBy">ID người nhúng</param>
	/// <param name="offset">Vị trí bắt đầu</param>
	/// <param name="limit">Số lượng bản ghi</param>
	Task<IEnumerable<Comic>> GetByEmbeddedByAsync(long embeddedBy, int offset = 0, int limit = 50);

	/// <summary>
	/// Lấy comic theo id và người nhúng
	/// </summary>
	Task<Comic?> GetByIdAndEmbeddedByAsync(long id, long embeddedBy);

	/// <summary>
	/// Tìm kiếm comic theo từ khóa
	/// </summary>
	/// <param name="vector">Vector truy vấn, null nếu không thể sinh embedding</param>
	/// <param name="keyword">Từ khóa tìm kiếm</param>
	/// <param name="limit">Giới hạn kết quả</param>
	/// <param name="minScore">Ngưỡng điểm tương đồng (0-1)</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<Comic>> SearchAsync(Vector? vector, string keyword, int limit, double minScore);

	/// <summary>
	/// Lấy danh sách comic theo tác giả
	/// </summary>
	/// <param name="author">Tên tác giả</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<Comic>> GetByAuthorAsync(string author);

	/// <summary>
	/// Lấy danh sách comic theo trạng thái
	/// </summary>
	/// <param name="status">Trạng thái comic</param>
	/// <returns>Danh sách comic</returns>
	Task<IEnumerable<Comic>> GetByStatusAsync(ComicStatus status);

	/// <summary>
	/// Lấy danh sách comic có điểm đánh giá cao nhất
	/// </summary>
	/// <param name="limit">Số lượng comic cần lấy</param>
	/// <returns>Danh sách comic theo điểm đánh giá</returns>
	Task<IEnumerable<Comic>> GetTopRatedAsync(int limit);

	/// <summary>
	/// Tổng số lượt bookmark của toàn bộ comic
	/// </summary>
	/// <returns>Tổng lượt bookmark</returns>
	Task<long> SumBookmarkCountAsync();
}
