using TruyenCV;

namespace TruyenCV.DTOs.Response;

public class ComicReportResponse
{
    public string id { get; set; }
    public string comic_id { get; set; }
    public string? chapter_id { get; set; }
    public string? comment_id { get; set; }
    public string reporter_id { get; set; }
    public string reason { get; set; }
    public ReportStatus status { get; set; }
    public string? comic_name { get; set; }
    public ComicStatus? comic_status { get; set; }
    public string? reporter_email { get; set; }
    public string? reporter_name { get; set; }
    public string? comment_content { get; set; }
    public bool? comment_is_hidden { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
