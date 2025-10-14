using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

public interface IUserHasSubscriptionService
{
    Task<UserSubscriptionResponse?> GetByIdAsync(long id);
    Task<IEnumerable<UserSubscriptionResponse>> GetByUserIdAsync(long userId);
    Task<UserSubscriptionResponse> CreateAsync(CreateUserSubscriptionRequest request);
    Task<UserSubscriptionResponse?> UpdateAsync(long id, UpdateUserSubscriptionRequest request);
    Task<bool> DeleteAsync(long id);
}
