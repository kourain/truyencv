
namespace TruyenCV.DTOs.Request;

public class CreateUserUseCoinHistoryRequest
{
    public required string user_id { get; set; }
    public long coin { get; set; }
    public HistoryStatus status { get; set; }
    public string? reason { get; set; }
    public string? reference_id { get; set; }
    public string? reference_type { get; set; }
}
