using System.Linq;
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
        return reports.Select(r => r.ToRespDTO());
    }

    public async Task<IEnumerable<ComicReportResponse>> GetReportsByUserAsync(long userId, int offset, int limit)
    {
        var reports = await _reportRepository.GetByUserIdAsync(userId, offset, limit);
        return reports.Select(r => r.ToRespDTO());
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
        return report.ToRespDTO();
    }

    public async Task<ComicReportResponse?> GetByIdAsync(long id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        return report?.ToRespDTO();
    }

    private async Task EnsureComicExists(long comicId)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            throw new UserRequestException("Không tìm thấy truyện");
        }
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
}
