using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IUserUseCoinHistoryService
{
    Task<IEnumerable<UserUseCoinHistoryResponse>> GetByUserIdAsync(long userId);
    Task<UserUseCoinHistoryResponse> CreateAsync(CreateUserUseCoinHistoryRequest request);
}
