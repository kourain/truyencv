using System.Linq;
using Microsoft.EntityFrameworkCore;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class ComicReportService : IComicReportService
{
    private readonly IComicReportRepository _reportRepository;
    private readonly IComicRepository _comicRepository;
    private readonly IComicChapterRepository _chapterRepository;
    private readonly IComicCommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public ComicReportService(
        IComicReportRepository reportRepository,
        IComicRepository comicRepository,
        IComicChapterRepository chapterRepository,
        IComicCommentRepository commentRepository,
        IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _comicRepository = comicRepository;
        _chapterRepository = chapterRepository;
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<ComicReportResponse> CreateReportAsync(CreateComicReportRequest request, long reporterId)
    {
        await EnsureReporterExists(reporterId);

        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        await EnsureComicExists(comicId);

        var chapterId = request.chapter_id.ToNullableSnowflakeId(nameof(request.chapter_id));
        var commentId = request.comment_id.ToNullableSnowflakeId(nameof(request.comment_id));

        await ValidateRelations(comicId, chapterId, commentId);

        var entity = request.ToEntity(reporterId);
        entity.comic_id = comicId;
        entity.chapter_id = chapterId;
        entity.comment_id = commentId;

        var created = await _reportRepository.AddAsync(entity);
        return created.ToRespDTO();
    }

    public async Task<IEnumerable<ComicReportResponse>> GetReportsAsync(int offset, int limit, ReportStatus? status = null)
    {
        var reports = await _reportRepository.GetByStatusAsync(status, offset, limit);
        return await MapReportsAsync(reports);
    }

    public async Task<IEnumerable<ComicReportResponse>> GetReportsByUserAsync(long userId, int offset, int limit)
    {
        var reports = await _reportRepository.GetByUserIdAsync(userId, offset, limit);
        return await MapReportsAsync(reports);
    }

    public async Task<IEnumerable<ComicReportResponse>> GetReportsByComicOwnerAsync(long userId, int offset, int limit, ReportStatus? status = null)
    {
        var reports = await _reportRepository.GetByComicOwnerAsync(userId, offset, limit, status);
        return await MapReportsAsync(reports);
    }

    public async Task<ComicReportResponse?> UpdateStatusAsync(long id, ReportStatus status)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return null;
        }

        report.UpdateStatus(status);
        await _reportRepository.UpdateAsync(report);
        return await MapReportAsync(report);
    }

    public async Task<ComicReportResponse?> GetByIdAsync(long id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return null;
        }

        return await MapReportAsync(report);
    }

    private async Task EnsureComicExists(long comicId)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            throw new UserRequestException("Không tìm thấy truyện");
        }
    }

    public async Task<ComicReportResponse?> BanComicAsync(long id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return null;
        }

        var comic = await _comicRepository.GetByIdAsync(report.comic_id);
        if (comic == null)
        {
            throw new UserRequestException("Không tìm thấy truyện để cấm");
        }

        if (comic.status != ComicStatus.Banned)
        {
            comic.status = ComicStatus.Banned;
            await _comicRepository.UpdateAsync(comic);
        }

        report.UpdateStatus(ReportStatus.Resolved);
        await _reportRepository.UpdateAsync(report);

        return await MapReportAsync(report);
    }

    public async Task<ComicReportResponse?> HideCommentAsync(long id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return null;
        }

        if (!report.comment_id.HasValue)
        {
            throw new UserRequestException("Báo cáo này không gắn với bình luận");
        }

        var comment = await _commentRepository.GetByIdAsync(report.comment_id.Value);
        if (comment == null)
        {
            throw new UserRequestException("Không tìm thấy bình luận để ẩn");
        }

        if (!comment.is_hidden)
        {
            comment.is_hidden = true;
            await _commentRepository.UpdateAsync(comment);
        }

        report.UpdateStatus(ReportStatus.Resolved);
        await _reportRepository.UpdateAsync(report);

        return await MapReportAsync(report);
    }

    private async Task EnsureReporterExists(long reporterId)
    {
        var reporter = await _userRepository.GetByIdAsync(reporterId);
        if (reporter == null)
        {
            throw new UserRequestException("Người dùng không hợp lệ");
        }
    }

    private async Task ValidateRelations(long comicId, long? chapterId, long? commentId)
    {
        if (!chapterId.HasValue && commentId.HasValue)
        {
            throw new UserRequestException("Thiếu thông tin chapter cho báo cáo bình luận");
        }

        if (chapterId.HasValue)
        {
            var chapter = await _chapterRepository.GetByIdAsync(chapterId.Value);
            if (chapter == null || chapter.comic_id != comicId)
            {
                throw new UserRequestException("Chapter không hợp lệ");
            }
        }

        if (commentId.HasValue)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId.Value);
            if (comment == null)
            {
                throw new UserRequestException("Bình luận không tồn tại");
            }

            if (chapterId.HasValue && comment.comic_chapter_id != chapterId)
            {
                throw new UserRequestException("Bình luận không thuộc chapter này");
            }

            if (comment.comic_id != comicId)
            {
                throw new UserRequestException("Bình luận không thuộc truyện này");
            }
        }
    }

    private async Task<IEnumerable<ComicReportResponse>> MapReportsAsync(IEnumerable<ComicReport> reports)
    {
        var reportList = reports.ToList();
        if (reportList.Count == 0)
        {
            return Array.Empty<ComicReportResponse>();
        }

        var comicIds = reportList.Select(r => r.comic_id).ToHashSet();
        var commentIds = reportList.Where(r => r.comment_id.HasValue).Select(r => r.comment_id!.Value).ToHashSet();
        var reporterIds = reportList.Select(r => r.reporter_id).ToHashSet();

        var comics = await _comicRepository.FindAsync(c => comicIds.Contains(c.id));
        var comments = commentIds.Count == 0
            ? Enumerable.Empty<ComicComment>()
            : await _commentRepository.FindAsync(c => commentIds.Contains(c.id));
        var reporters = await _userRepository.FindAsync(u => reporterIds.Contains(u.id));

        var comicDict = comics.ToDictionary(c => c.id, c => c);
        var commentDict = comments.ToDictionary(c => c.id, c => c);
        var reporterDict = reporters.ToDictionary(u => u.id, u => u);

        return reportList.Select(report => MapReport(report, comicDict, commentDict, reporterDict)).ToList();
    }

    private async Task<ComicReportResponse> MapReportAsync(ComicReport report)
    {
        var result = await MapReportsAsync(new[] { report });
        return result.First();
    }

    private static ComicReportResponse MapReport(
        ComicReport report,
        Dictionary<long, Comic> comicDict,
        Dictionary<long, ComicComment> commentDict,
        Dictionary<long, User> reporterDict)
    {
        var response = report.ToRespDTO();

        if (comicDict.TryGetValue(report.comic_id, out var comic))
        {
            response.comic_name = comic.name;
            response.comic_status = comic.status;
        }

        if (report.comment_id.HasValue && commentDict.TryGetValue(report.comment_id.Value, out var comment))
        {
            response.comment_content = comment.comment;
            response.comment_is_hidden = comment.is_hidden;
        }

        if (reporterDict.TryGetValue(report.reporter_id, out var reporter))
        {
            response.reporter_email = reporter.email;
            response.reporter_name = reporter.name;
        }

        return response;
    }
}
