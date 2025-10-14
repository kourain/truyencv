using System.Linq;
using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class PaymentHistoryService : IPaymentHistoryService
{
    private readonly IPaymentHistoryRepository _paymentHistoryRepository;

    public PaymentHistoryService(IPaymentHistoryRepository paymentHistoryRepository)
    {
        _paymentHistoryRepository = paymentHistoryRepository;
    }

    public async Task<PaymentHistoryResponse?> GetByIdAsync(long id)
    {
        var history = await _paymentHistoryRepository.FirstOrDefaultAsync(entity => entity.id == id && entity.deleted_at == null);
        return history?.ToRespDTO();
    }

    public async Task<IEnumerable<PaymentHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = await _paymentHistoryRepository.GetByUserIdAsync(userId);
        return histories.Where(history => history.deleted_at == null).Select(history => history.ToRespDTO());
    }

    public async Task<IEnumerable<PaymentHistoryResponse>> GetAsync(int offset, int limit)
    {
        var histories = await _paymentHistoryRepository.GetPagedAsync(offset, limit);
        return histories.Where(history => history.deleted_at == null).Select(history => history.ToRespDTO());
    }

    public async Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _paymentHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }
}
