using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class ComicReportController : ControllerBase
{
    private readonly IComicReportService _reportService;
    private readonly IComicService _comicService;

    public ComicReportController(IComicReportService reportService, IComicService comicService)
    {
        _reportService = reportService;
        _comicService = comicService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyReports([FromQuery] int offset = 0, [FromQuery] int limit = 20, [FromQuery] ReportStatus? status = null)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var reports = await _reportService.GetReportsByComicOwnerAsync(userId.Value, offset, limit, status);
        return Ok(reports);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var report = await _reportService.GetByIdAsync(id);
        if (report == null)
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        var parsedComicId = report.comic_id.ToSnowflakeId();
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        return Ok(report);
    }
}
