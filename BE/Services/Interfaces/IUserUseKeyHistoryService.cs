using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

public interface IUserUseKeyHistoryService
{
    Task<IEnumerable<UserUseKeyHistoryResponse>> GetByUserIdAsync(long userId);
    Task<UserUseKeyHistoryResponse> CreateAsync(CreateUserUseKeyHistoryRequest request);
}
