using System;

namespace TruyenCV.DTOs.Response;

public class PaymentHistoryResponse
{
    public required string id { get; set; }
    public required string user_id { get; set; }
    public long amount_coin { get; set; }
    public long amount_money { get; set; }
    public string payment_method { get; set; } = "unknown";
    public string? reference_id { get; set; }
    public string? note { get; set; }
    public string? user_email { get; set; }
    public string? user_name { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

public class PaymentRevenuePointResponse
{
    public required string date { get; set; }
    public long total_amount_coin { get; set; }
    public long total_amount_money { get; set; }
}
