using System;

namespace TruyenCV.DTO.Response;

public class UserSubscriptionResponse
{
    public required string id { get; set; }
    public required string user_id { get; set; }
    public required string subscription_id { get; set; }
    public DateTime start_at { get; set; }
    public DateTime? end_at { get; set; }
    public bool is_active { get; set; }
    public bool auto_renew { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
