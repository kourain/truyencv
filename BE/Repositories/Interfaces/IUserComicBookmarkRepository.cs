using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserComicBookmark entity
/// </summary>
public interface IUserComicBookmarkRepository : IRepository<UserComicBookmark>
{
    /// <summary>
    /// Lấy bookmark theo id
    /// </summary>
    Task<UserComicBookmark?> GetByIdAsync(ulong id);

    /// <summary>
    /// Lấy bookmark theo user và comic
    /// </summary>
    Task<UserComicBookmark?> GetByUserAndComicAsync(ulong userId, ulong comicId);

    /// <summary>
    /// Lấy danh sách bookmark theo user
    /// </summary>
    Task<IEnumerable<UserComicBookmark>> GetByUserIdAsync(ulong userId);

    /// <summary>
    /// Lấy danh sách bookmark theo comic
    /// </summary>
    Task<IEnumerable<UserComicBookmark>> GetByComicIdAsync(ulong comicId);
}
