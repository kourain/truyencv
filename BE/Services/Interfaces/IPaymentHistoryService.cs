using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IPaymentHistoryService
{
    Task<PaymentHistoryResponse?> GetByIdAsync(long id);
    Task<IEnumerable<PaymentHistoryResponse>> GetByUserIdAsync(long userId);
    Task<IEnumerable<PaymentHistoryResponse>> GetAsync(int offset, int limit, string? keyword = null);
    Task<PaymentHistoryResponse> CreateAsync(CreatePaymentHistoryRequest request);
    Task<IEnumerable<PaymentRevenuePointResponse>> GetRevenueSummaryAsync(int days = 60);
}
