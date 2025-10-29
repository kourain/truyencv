using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserUseCoinHistoryRepository : IRepository<UserUseCoinHistory>
{
    Task<IEnumerable<UserUseCoinHistory>> GetByUserIdAsync(long userId);
}
