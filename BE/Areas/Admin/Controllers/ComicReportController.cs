using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class ComicReportController : ControllerBase
{
    private readonly IComicReportService _reportService;

    public ComicReportController(IComicReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReports([FromQuery] int offset = 0, [FromQuery] int limit = 20, [FromQuery] ReportStatus? status = null)
    {
        var reports = await _reportService.GetReportsAsync(offset, limit, status);
        return Ok(reports);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var report = await _reportService.GetByIdAsync(id);
        if (report == null)
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        return Ok(report);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateComicReportStatusRequest request)
    {
        var requestId = request.id.ToSnowflakeId(nameof(request.id));
        if (requestId != id)
        {
            return BadRequest(new { message = "ID không khớp" });
        }

        var updated = await _reportService.UpdateStatusAsync(id, request.status);
        if (updated == null)
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        return Ok(updated);
    }

    [HttpPost("{id}/ban-comic")]
    public async Task<IActionResult> BanComic(long id)
    {
        var result = await _reportService.BanComicAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        return Ok(result);
    }

    [HttpPost("{id}/hide-comment")]
    public async Task<IActionResult> HideComment(long id)
    {
        var result = await _reportService.HideCommentAsync(id);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy báo cáo" });
        }

        return Ok(result);
    }
}
