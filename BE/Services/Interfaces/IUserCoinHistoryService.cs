using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IUserCoinHistoryService
{
    Task<IEnumerable<UserCoinHistoryResponse>> GetByUserIdAsync(long userId);
    Task<UserCoinHistoryResponse> CreateAsync(CreateUserCoinHistoryRequest request);
}
