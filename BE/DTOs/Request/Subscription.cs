namespace TruyenCV.DTOs.Request;

public class CreateSubscriptionRequest
{
    public required string code { get; set; }
    public required string name { get; set; }
    public string? description { get; set; }
    public long price_coin { get; set; }
    public int duration_day { get; set; }
    public bool is_active { get; set; } = true;
}

public class UpdateSubscriptionRequest
{
    public required string id { get; set; }
    public required string code { get; set; }
    public required string name { get; set; }
    public string? description { get; set; }
    public long price_coin { get; set; }
    public int duration_day { get; set; }
    public bool is_active { get; set; } = true;
}
