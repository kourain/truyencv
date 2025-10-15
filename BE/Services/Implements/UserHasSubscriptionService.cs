using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserHasSubscriptionService : IUserHasSubscriptionService
{
    private readonly IUserHasSubscriptionRepository _userHasSubscriptionRepository;

    public UserHasSubscriptionService(IUserHasSubscriptionRepository userHasSubscriptionRepository)
    {
        _userHasSubscriptionRepository = userHasSubscriptionRepository;
    }

    public async Task<UserSubscriptionResponse?> GetByIdAsync(long id)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        return subscription?.ToRespDTO();
    }

    public async Task<IEnumerable<UserSubscriptionResponse>> GetByUserIdAsync(long userId)
    {
        var subscriptions = await _userHasSubscriptionRepository.GetByUserIdAsync(userId);
        return subscriptions.Where(sub => sub.deleted_at == null).Select(sub => sub.ToRespDTO());
    }

    public async Task<UserSubscriptionResponse> CreateAsync(CreateUserSubscriptionRequest request)
    {
        var entity = request.ToEntity();
        var created = await _userHasSubscriptionRepository.AddAsync(entity);
        return created.ToRespDTO();
    }

    public async Task<UserSubscriptionResponse?> UpdateAsync(long id, UpdateUserSubscriptionRequest request)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return null;
        }

        subscription.UpdateFromRequest(request);
        await _userHasSubscriptionRepository.UpdateAsync(subscription);
        return subscription.ToRespDTO();
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var subscription = await _userHasSubscriptionRepository.FirstOrDefaultAsync(sub => sub.id == id && sub.deleted_at == null);
        if (subscription == null)
        {
            return false;
        }

        await _userHasSubscriptionRepository.DeleteAsync(subscription);
        return true;
    }
}
