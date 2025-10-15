using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserComicBookmark Service
/// </summary>
public interface IUserComicBookmarkService
{
    /// <summary>
    /// Lấy danh sách bookmark của user
    /// </summary>
    Task<IEnumerable<UserComicBookmarkResponse>> GetBookmarksByUserIdAsync(long userId);

    /// <summary>
    /// Tạo bookmark mới
    /// </summary>
    Task<UserComicBookmarkResponse> CreateBookmarkAsync(long userId, long comicId);

    /// <summary>
    /// Kiểm tra user đã bookmark comic chưa
    /// </summary>
    Task<bool> IsBookmarkedAsync(long userId, long comicId);

    /// <summary>
    /// Xóa bookmark
    /// </summary>
    Task<bool> RemoveBookmarkAsync(long userId, long comicId);
}
