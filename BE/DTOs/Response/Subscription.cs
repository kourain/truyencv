using System;

namespace TruyenCV.DTOs.Response;

public class SubscriptionResponse
{
    public required string id { get; set; }
    public required string code { get; set; }
    public required string name { get; set; }
    public string? description { get; set; }
    public long price_coin { get; set; }
    public int duration_day { get; set; }
    public bool is_active { get; set; }
    public int ticket_added { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
