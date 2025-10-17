using TruyenCV;

namespace TruyenCV.DTOs.Request;

public class CreateComicReportRequest
{
    public required string comic_id { get; set; }
    public string? chapter_id { get; set; }
    public string? comment_id { get; set; }
    public required string reason { get; set; }
}

public class UpdateComicReportStatusRequest
{
    public required string id { get; set; }
    public required ReportStatus status { get; set; }
}
