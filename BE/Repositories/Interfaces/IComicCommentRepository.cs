using System.Collections.Generic;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho ComicComment entity
/// </summary>
public interface IComicCommentRepository : IRepository<ComicComment>
{

	/// <summary>
	/// Lấy danh sách comment của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByComicIdAsync(long comicId);

	/// <summary>
	/// Lấy danh sách comment của một chapter
	/// </summary>
	/// <param name="chapterId">ID của chapter</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByChapterIdAsync(long chapterId);

	/// <summary>
	/// Lấy danh sách comment của một user
	/// </summary>
	/// <param name="userId">ID của user</param>
	/// <returns>Danh sách comment</returns>
	Task<IEnumerable<ComicComment>> GetByUserIdAsync(long userId);

	/// <summary>
	/// Lấy danh sách reply của một comment
	/// </summary>
	/// <param name="commentId">ID của comment</param>
	/// <returns>Danh sách reply</returns>
	Task<IEnumerable<ComicComment>> GetRepliesAsync(long commentId);

	/// <summary>
	/// Lấy danh sách đánh giá mới nhất
	/// </summary>
	/// <param name="limit">Số lượng đánh giá cần lấy</param>
	Task<IEnumerable<ComicComment>> GetLatestRatingReviewsAsync(int limit);
}
