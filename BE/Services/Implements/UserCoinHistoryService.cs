using System.Linq;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserCoinHistoryService : IUserCoinHistoryService
{
    private readonly IUserCoinHistoryRepository _userCoinHistoryRepository;

    public UserCoinHistoryService(IUserCoinHistoryRepository userCoinHistoryRepository)
    {
        _userCoinHistoryRepository = userCoinHistoryRepository;
    }

    public async Task<IEnumerable<UserCoinHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _userCoinHistoryRepository.GetByUserIdAsync(userId);
        return histories.Where(history => history.deleted_at == null).Select(history => history.ToRespDTO());
    }

    public async Task<UserCoinHistoryResponse> CreateAsync(CreateUserCoinHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _userCoinHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }
}
