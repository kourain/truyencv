using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

public interface IPaymentHistoryService
{
    Task<PaymentHistoryResponse?> GetByIdAsync(long id);
    Task<IEnumerable<PaymentHistoryResponse>> GetByUserIdAsync(long userId);
    Task<IEnumerable<PaymentHistoryResponse>> GetAsync(int offset, int limit);
    Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request);
}
