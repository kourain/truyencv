using System.Collections.Generic;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho ComicChapter entity
/// </summary>
public interface IComicChapterRepository : IRepository<ComicChapter>
{
	/// <summary>
	/// Lấy chapter theo id
	/// </summary>
	/// <param name="id">ID của chapter</param>
	/// <returns>Chapter nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicChapter?> GetByIdAsync(long id);

	/// <summary>
	/// Lấy danh sách chapter của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách chapter</returns>
	Task<IEnumerable<ComicChapter>> GetByComicIdAsync(long comicId);

	/// <summary>
	/// Lấy chapter theo comic id và số chapter
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="chapter">Số chapter</param>
	/// <returns>Chapter nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicChapter?> GetByComicIdAndChapterAsync(long comicId, int chapter);

	/// <summary>
	/// Lấy chapter trước đó của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="chapter">Số chapter hiện tại</param>
	/// <returns>Chapter nếu tìm thấy, null nếu không</returns>
	Task<ComicChapter?> GetPreviousChapterAsync(long comicId, int chapter);

	/// <summary>
	/// Lấy chapter tiếp theo của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="chapter">Số chapter hiện tại</param>
	/// <returns>Chapter nếu tìm thấy, null nếu không</returns>
	Task<ComicChapter?> GetNextChapterAsync(long comicId, int chapter);

	/// <summary>
	/// Lấy danh sách chapter được cập nhật mới nhất
	/// </summary>
	/// <param name="limit">Số lượng chapter cần lấy</param>
	Task<IEnumerable<ComicChapter>> GetLatestUpdatedAsync(int limit);
}
