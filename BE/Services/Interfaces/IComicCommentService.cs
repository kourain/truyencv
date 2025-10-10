using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho ComicComment Service
/// </summary>
public interface IComicCommentService
{
	/// <summary>
	/// Lấy thông tin comment theo ID
	/// </summary>
	/// <param name="id">ID của comment</param>
	/// <returns>Thông tin comment</returns>
	Task<ComicCommentResponse?> GetCommentByIdAsync(ulong id);

	/// <summary>
	/// Lấy danh sách comment của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicCommentResponse>> GetCommentsByComicIdAsync(ulong comicId);

	/// <summary>
	/// Lấy danh sách comment của một chapter
	/// </summary>
	/// <param name="chapterId">ID của chapter</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicCommentResponse>> GetCommentsByChapterIdAsync(ulong chapterId);

	/// <summary>
	/// Lấy danh sách comment của một user
	/// </summary>
	/// <param name="userId">ID của user</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicCommentResponse>> GetCommentsByUserIdAsync(ulong userId);

	/// <summary>
	/// Lấy danh sách reply của một comment
	/// </summary>
	/// <param name="commentId">ID của comment</param>
	/// <returns>Danh sách reply</returns>
	Task<IEnumerable<ComicCommentResponse>> GetRepliesAsync(ulong commentId);

	/// <summary>
	/// Tạo comment mới
	/// </summary>
	/// <param name="commentRequest">Thông tin comment mới</param>
	/// <returns>Thông tin comment đã tạo</returns>
	Task<ComicCommentResponse> CreateCommentAsync(CreateComicCommentRequest commentRequest);

	/// <summary>
	/// Cập nhật thông tin comment
	/// </summary>
	/// <param name="id">ID của comment</param>
	/// <param name="commentRequest">Thông tin cập nhật</param>
	/// <returns>Thông tin comment đã cập nhật</returns>
	Task<ComicCommentResponse?> UpdateCommentAsync(ulong id, UpdateComicCommentRequest commentRequest);

	/// <summary>
	/// Xóa comment
	/// </summary>
	/// <param name="id">ID của comment</param>
	/// <returns>True nếu xóa thành công, ngược lại là False</returns>
	Task<bool> DeleteCommentAsync(ulong id);
}
