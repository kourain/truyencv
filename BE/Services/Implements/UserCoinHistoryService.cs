using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserUseCoinHistoryService : IUserUseCoinHistoryService
{
    private readonly IUserUseCoinHistoryRepository _UserUseCoinHistoryRepository;

    public UserUseCoinHistoryService(IUserUseCoinHistoryRepository UserUseCoinHistoryRepository)
    {
        _UserUseCoinHistoryRepository = UserUseCoinHistoryRepository;
    }

    public async Task<IEnumerable<UserUseCoinHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _UserUseCoinHistoryRepository.GetByUserIdAsync(userId);
        return histories.Where(history => history.deleted_at == null).Select(history => history.ToRespDTO());
    }

    public async Task<UserUseCoinHistoryResponse> CreateAsync(CreateUserUseCoinHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _UserUseCoinHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }
}
