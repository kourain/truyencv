using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserComicReadHistory entity
/// </summary>
public interface IUserComicReadHistoryRepository : IRepository<UserComicReadHistory>
{
    /// <summary>
    /// Lấy bản ghi theo id
    /// </summary>
    Task<UserComicReadHistory?> GetByIdAsync(long id);

    /// <summary>
    /// Lấy bản ghi đọc theo user và comic
    /// </summary>
    Task<UserComicReadHistory?> GetByUserAndComicAsync(long userId, long comicId);

    /// <summary>
    /// Lấy danh sách lịch sử đọc của user
    /// </summary>
    Task<IEnumerable<UserComicReadHistory>> GetByUserIdAsync(long userId, int limit);
}
