using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserComicRecommendRepository : IRepository<UserComicRecommend>
{
    Task<UserComicRecommend?> GetByUserAndPeriodAsync(long userId, int month, int year);
}
