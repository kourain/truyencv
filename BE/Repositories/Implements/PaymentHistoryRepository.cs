using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.Models;

namespace TruyenCV.Repositories;

public class PaymentHistoryRepository : Repository<PaymentHistory>, IPaymentHistoryRepository
{
    public PaymentHistoryRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<IEnumerable<PaymentHistory>> GetByUserIdAsync(long userId)
    {
        var result = await _redisCache.GetFromRedisAsync<PaymentHistory>(
            () => _dbSet.AsNoTracking()
                .Where(history => history.user_id == userId && history.deleted_at == null)
                .OrderByDescending(history => history.created_at)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
        return result ?? Enumerable.Empty<PaymentHistory>();
    }

    public async Task<IEnumerable<PaymentHistory>> GetPagedByUserIdsAsync(IEnumerable<long> userIds, int offset, int limit)
    {
        var ids = userIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return Enumerable.Empty<PaymentHistory>();
        }

        offset = Math.Max(offset, 0);
        limit = Math.Clamp(limit, 1, 200);

        return await _dbSet.AsNoTracking()
            .Where(history => ids.Contains(history.user_id) && history.deleted_at == null)
            .OrderByDescending(history => history.created_at)
            .ThenByDescending(history => history.id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentHistoryDailyAggregate>> GetDailyRevenueAsync(DateTime fromUtc, DateTime toUtc)
    {
        return await _dbSet.AsNoTracking()
            .Where(history => history.deleted_at == null && history.created_at >= fromUtc && history.created_at <= toUtc)
            .GroupBy(history => history.created_at.Date)
            .Select(group => new PaymentHistoryDailyAggregate(
                group.Key,
                group.Sum(item => item.amount_coin),
                group.Sum(item => item.amount_money)))
            .OrderBy(result => result.Date)
            .ToListAsync();
    }
}
