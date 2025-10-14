using System;
using TruyenCV.Const;

namespace TruyenCV.DTO.Response;

public class UserCoinHistoryResponse
{
    public required string id { get; set; }
    public required string user_id { get; set; }
    public long coin { get; set; }
    public HistoryStatus status { get; set; }
    public string? reason { get; set; }
    public string? reference_id { get; set; }
    public string? reference_type { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
