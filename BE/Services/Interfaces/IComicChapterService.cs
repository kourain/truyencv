using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho ComicChapter Service
/// </summary>
public interface IComicChapterService
{
	/// <summary>
	/// Lấy thông tin chapter theo ID
	/// </summary>
	/// <param name="id">ID của chapter</param>
	/// <returns>Thông tin chapter</returns>
	Task<ComicChapterResponse?> GetChapterByIdAsync(long id);

	/// <summary>
	/// Lấy danh sách chapter của một comic
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <returns>Danh sách chapter</returns>
	Task<IEnumerable<ComicChapterResponse>> GetChaptersByComicIdAsync(long comicId);

	/// <summary>
	/// Lấy chapter theo comic id và số chapter
	/// </summary>
	/// <param name="comicId">ID của comic</param>
	/// <param name="chapter">Số chapter</param>
	/// <returns>Thông tin chapter</returns>
	Task<ComicChapterResponse?> GetChapterByComicIdAndChapterAsync(long comicId, int chapter);

	/// <summary>
	/// Tạo chapter mới
	/// </summary>
	/// <param name="chapterRequest">Thông tin chapter mới</param>
	/// <returns>Thông tin chapter đã tạo</returns>
	Task<ComicChapterResponse> CreateChapterAsync(CreateComicChapterRequest chapterRequest);

	/// <summary>
	/// Cập nhật thông tin chapter
	/// </summary>
	/// <param name="id">ID của chapter</param>
	/// <param name="chapterRequest">Thông tin cập nhật</param>
	/// <returns>Thông tin chapter đã cập nhật</returns>
	Task<ComicChapterResponse?> UpdateChapterAsync(long id, UpdateComicChapterRequest chapterRequest);

	/// <summary>
	/// Xóa chapter
	/// </summary>
	/// <param name="id">ID của chapter</param>
	/// <returns>True nếu xóa thành công, ngược lại là False</returns>
	Task<bool> DeleteChapterAsync(long id);
}
