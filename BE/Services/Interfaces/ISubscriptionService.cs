using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface ISubscriptionService
{
    Task<SubscriptionResponse?> GetByIdAsync(long id);
    Task<IEnumerable<SubscriptionResponse>> GetAsync(int offset, int limit);
    Task<SubscriptionResponse> CreateAsync(CreateSubscriptionRequest request);
    Task<SubscriptionResponse?> UpdateAsync(long id, UpdateSubscriptionRequest request);
    Task<bool> DeleteAsync(long id);
}
