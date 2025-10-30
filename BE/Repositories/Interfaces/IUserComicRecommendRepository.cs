using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserComicRecommendRepository : IRepository<UserComicRecommend>
{
    Task<UserComicRecommend?> GetByIdAsync(long id);
    Task<UserComicRecommend?> GetByUserAndPeriodAsync(long userId, int month, int year);
}
