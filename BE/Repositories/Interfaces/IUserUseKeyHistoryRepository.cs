using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserUseKeyHistoryRepository : IRepository<UserUseKeyHistory>
{
    Task<IEnumerable<UserUseKeyHistory>> GetByUserIdAsync(long userId);
}
