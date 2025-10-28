using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserComicUnlockHistoryRepository : IRepository<UserComicUnlockHistory>
{
    Task<UserComicUnlockHistory?> GetByUserAndChapterAsync(long userId, long chapterId);
    Task<IEnumerable<UserComicUnlockHistory>> GetByUserIdAsync(long userId);
    Task<IEnumerable<UserComicUnlockHistory>> GetByComicIdAsync(long comicId);
}
