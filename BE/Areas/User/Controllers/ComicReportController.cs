using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class ComicReportController : ControllerBase
{
    private readonly IComicReportService _reportService;

    public ComicReportController(IComicReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyReports([FromQuery] int offset = 0, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không xác định được người dùng" });
        }

        var reports = await _reportService.GetReportsByUserAsync(userId.Value, offset, limit);
        return Ok(reports);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComicReportRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không xác định được người dùng" });
        }

        var report = await _reportService.CreateReportAsync(request, userId.Value);
        return Ok(report);
    }
}
