using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IPaymentHistoryRepository : IRepository<PaymentHistory>
{
    Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(long userId);
}
