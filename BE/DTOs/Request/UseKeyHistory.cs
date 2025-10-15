
namespace TruyenCV.DTOs.Request;

public class CreateUserUseKeyHistoryRequest
{
    public required string user_id { get; set; }
    public long key { get; set; }
    public HistoryStatus status { get; set; }
    public string? chapter_id { get; set; }
    public string? note { get; set; }
}
