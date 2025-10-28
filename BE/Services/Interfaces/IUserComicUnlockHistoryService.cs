using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IUserComicUnlockHistoryService
{
    Task<IEnumerable<UserComicUnlockHistoryResponse>> GetByUserIdAsync(long userId);
    Task<IEnumerable<UserComicUnlockHistoryResponse>> GetByComicIdAsync(long comicId);
    Task<UserComicUnlockHistoryResponse> CreateAsync(CreateUserComicUnlockHistoryRequest request);
    Task<UserComicUnlockHistoryResponse> UnlockChapterAsync(long userId, UnlockComicChapterRequest request);
    Task<bool> HasUnlockedChapterAsync(long userId, long chapterId);
}
