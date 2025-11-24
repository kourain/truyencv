using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static ComicReport ToEntity(this CreateComicReportRequest request, long reporterId)
    {
        return new ComicReport
        {
            comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
            chapter_id = request.chapter_id.ToNullableSnowflakeId(nameof(request.chapter_id)),
            comment_id = request.comment_id.ToNullableSnowflakeId(nameof(request.comment_id)),
            reporter_id = reporterId,
            reason = request.reason,
            status = ReportStatus.Pending
        };
    }

    public static ComicReportResponse ToRespDTO(this ComicReport report)
    {
        return new ComicReportResponse
        {
            id = report._id,
            comic_id = report.comic_id.ToString(),
            chapter_id = report.chapter_id?.ToString(),
            comment_id = report.comment_id?.ToString(),
            reporter_id = report.reporter_id.ToString(),
            reason = report.reason,
            status = report.status,
            comic_name = null,
            comic_status = null,
            reporter_email = null,
            reporter_name = null,
            comment_content = null,
            comment_is_hidden = null,
            created_at = report.created_at,
            updated_at = report.updated_at
        };
    }

    public static void UpdateStatus(this ComicReport report, ReportStatus status)
    {
        report.status = status;
    }
}
