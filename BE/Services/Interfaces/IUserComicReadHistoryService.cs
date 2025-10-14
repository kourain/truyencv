using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserComicReadHistory Service
/// </summary>
public interface IUserComicReadHistoryService
{
    /// <summary>
    /// Lấy lịch sử đọc của user
    /// </summary>
    Task<IEnumerable<UserComicReadHistoryResponse>> GetReadHistoryByUserIdAsync(long userId, int limit = 20);

    /// <summary>
    /// Tạo hoặc cập nhật lịch sử đọc
    /// </summary>
    Task<UserComicReadHistoryResponse> UpsertReadHistoryAsync(long userId, long comicId, long chapterId);

    /// <summary>
    /// Xóa lịch sử đọc của một comic
    /// </summary>
    Task<bool> RemoveReadHistoryAsync(long userId, long comicId);
}
