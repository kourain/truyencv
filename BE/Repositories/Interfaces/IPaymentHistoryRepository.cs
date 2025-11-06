using System;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IPaymentHistoryRepository : IRepository<PaymentHistory>
{
    Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(long userId);
    Task<IEnumerable<PaymentHistory>> GetPagedByUserIdsAsync(IEnumerable<long> userIds, int offset, int limit);
    Task<IEnumerable<PaymentHistoryDailyAggregate>> GetDailyRevenueAsync(DateTime fromUtc, DateTime toUtc);
}

public record PaymentHistoryDailyAggregate(DateTime Date, long TotalCoin, long TotalMoney);
