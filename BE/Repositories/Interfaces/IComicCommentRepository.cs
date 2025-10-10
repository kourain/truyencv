using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho ComicComment entity
/// </summary>
public interface IComicCommentRepository : IRepository<ComicComment>
{
	/// <summary>
	/// Lấy comment theo id
	/// </summary>
	/// <param name="id">ID của comment</param>
	/// <returns>Comment nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicComment?> GetByIdAsync(ulong id);

	/// <summary>
	/// Lấy danh sách comment của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByComicIdAsync(ulong comicId);

	/// <summary>
	/// Lấy danh sách comment của một chapter
	/// </summary>
	/// <param name="chapterId">ID của chapter</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByChapterIdAsync(ulong chapterId);

	/// <summary>
	/// Lấy danh sách comment của một user
	/// </summary>
	/// <param name="userId">ID của user</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByUserIdAsync(ulong userId);

	/// <summary>
	/// Lấy danh sách reply của một comment
	/// </summary>
	/// <param name="commentId">ID của comment</param>
	/// <returns>Danh sách reply</returns>
	Task<IEnumerable<ComicComment>> GetRepliesAsync(ulong commentId);
}
