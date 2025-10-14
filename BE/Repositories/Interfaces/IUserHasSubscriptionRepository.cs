using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IUserHasSubscriptionRepository : IRepository<UserHasSubscription>
{
    Task<IEnumerable<UserHasSubscription>> GetByUserIdAsync(long userId);
}
