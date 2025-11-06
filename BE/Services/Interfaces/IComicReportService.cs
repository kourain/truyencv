using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV;

namespace TruyenCV.Services;

public interface IComicReportService
{
    Task<ComicReportResponse> CreateReportAsync(CreateComicReportRequest request, long reporterId);
    Task<IEnumerable<ComicReportResponse>> GetReportsAsync(int offset, int limit, ReportStatus? status = null);
    Task<IEnumerable<ComicReportResponse>> GetReportsByUserAsync(long userId, int offset, int limit);
    Task<ComicReportResponse?> UpdateStatusAsync(long id, ReportStatus status);
    Task<ComicReportResponse?> GetByIdAsync(long id);
    Task<ComicReportResponse?> BanComicAsync(long id);
    Task<ComicReportResponse?> HideCommentAsync(long id);
}
