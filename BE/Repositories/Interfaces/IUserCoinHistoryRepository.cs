using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserCoinHistoryRepository : IRepository<UserCoinHistory>
{
    Task<IEnumerable<UserCoinHistory>> GetByUserIdAsync(long userId);
}
