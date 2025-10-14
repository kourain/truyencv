using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

public interface IUserCoinHistoryService
{
    Task<IEnumerable<UserCoinHistoryResponse>> GetByUserIdAsync(long userId);
    Task<UserCoinHistoryResponse> CreateAsync(CreateUserCoinHistoryRequest request);
}
