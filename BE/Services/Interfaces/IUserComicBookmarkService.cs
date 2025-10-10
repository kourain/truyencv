using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserComicBookmark Service
/// </summary>
public interface IUserComicBookmarkService
{
    /// <summary>
    /// Lấy danh sách bookmark của user
    /// </summary>
    Task<IEnumerable<UserComicBookmarkResponse>> GetBookmarksByUserIdAsync(ulong userId);

    /// <summary>
    /// Tạo bookmark mới
    /// </summary>
    Task<UserComicBookmarkResponse> CreateBookmarkAsync(ulong userId, ulong comicId);

    /// <summary>
    /// Kiểm tra user đã bookmark comic chưa
    /// </summary>
    Task<bool> IsBookmarkedAsync(ulong userId, ulong comicId);

    /// <summary>
    /// Xóa bookmark
    /// </summary>
    Task<bool> RemoveBookmarkAsync(ulong userId, ulong comicId);
}
