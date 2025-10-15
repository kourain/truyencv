using System;

namespace TruyenCV.DTOs.Request;

public class CreateUserSubscriptionRequest
{
    public required string user_id { get; set; }
    public required string subscription_id { get; set; }
    public DateTime? start_at { get; set; }
    public DateTime? end_at { get; set; }
    public bool is_active { get; set; } = true;
    public bool auto_renew { get; set; } = false;
}

public class UpdateUserSubscriptionRequest
{
    public required string id { get; set; }
    public DateTime? end_at { get; set; }
    public bool is_active { get; set; } = true;
    public bool auto_renew { get; set; } = false;
}
