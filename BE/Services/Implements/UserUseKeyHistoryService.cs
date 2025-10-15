using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserUseKeyHistoryService : IUserUseKeyHistoryService
{
    private readonly IUserUseKeyHistoryRepository _userUseKeyHistoryRepository;

    public UserUseKeyHistoryService(IUserUseKeyHistoryRepository userUseKeyHistoryRepository)
    {
        _userUseKeyHistoryRepository = userUseKeyHistoryRepository;
    }

    public async Task<IEnumerable<UserUseKeyHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _userUseKeyHistoryRepository.GetByUserIdAsync(userId);
        return histories.Where(history => history.deleted_at == null).Select(history => history.ToRespDTO());
    }

    public async Task<UserUseKeyHistoryResponse> CreateAsync(CreateUserUseKeyHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _userUseKeyHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }
}
