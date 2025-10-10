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
	Task<ComicChapter?> GetByIdAsync(ulong id);

	/// <summary>
	/// Lấy danh sách chapter của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách chapter</returns>
	Task<IEnumerable<ComicChapter>> GetByComicIdAsync(ulong comicId);

	/// <summary>
	/// Lấy chapter theo comic id và số chapter
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="chapter">Số chapter</param>
	/// <returns>Chapter nếu tìm thấy, null nếu không tìm thấy</returns>
	Task<ComicChapter?> GetByComicIdAndChapterAsync(ulong comicId, int chapter);
}
