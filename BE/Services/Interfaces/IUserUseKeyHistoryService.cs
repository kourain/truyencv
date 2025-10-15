using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IUserUseKeyHistoryService
{
    Task<IEnumerable<UserUseKeyHistoryResponse>> GetByUserIdAsync(long userId);
    Task<UserUseKeyHistoryResponse> CreateAsync(CreateUserUseKeyHistoryRequest request);
}
