using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionResponse?> GetByIdAsync(long id)
    {
        var subscription = await _subscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        return subscription?.ToRespDTO();
    }

    public async Task<IEnumerable<SubscriptionResponse>> GetAsync(int offset, int limit)
    {
        var subscriptions = await _subscriptionRepository.GetPagedAsync(offset, limit);
        return subscriptions.Where(sub => sub.deleted_at == null).Select(sub => sub.ToRespDTO());
    }

    public async Task<SubscriptionResponse> CreateAsync(CreateSubscriptionRequest request)
    {
        var entity = request.ToEntity();
        var created = await _subscriptionRepository.AddAsync(entity);
        return created.ToRespDTO();
    }

    public async Task<SubscriptionResponse?> UpdateAsync(long id, UpdateSubscriptionRequest request)
    {
        var subscription = await _subscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return null;
        }

        subscription.UpdateFromRequest(request);
        await _subscriptionRepository.UpdateAsync(subscription);
        return subscription.ToRespDTO();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var subscription = await _subscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return false;
        }

        await _subscriptionRepository.DeleteAsync(subscription);
        return true;
    }
}
