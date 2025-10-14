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
}
