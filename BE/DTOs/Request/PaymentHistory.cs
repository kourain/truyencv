namespace TruyenCV.DTO.Request;

public class CreatePaymentHistoryRequest
{
    public required string user_id { get; set; }
    public long amount_coin { get; set; }
    public long amount_money { get; set; }
    public string payment_method { get; set; } = "unknown";
    public string? reference_id { get; set; }
    public string? note { get; set; }
}
