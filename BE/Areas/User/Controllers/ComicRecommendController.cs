using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Route("User/[controller]")]
public sealed class ComicRecommendController : ControllerBase
{
    private readonly IComicRecommendService _recommendService;

    public ComicRecommendController(IComicRecommendService recommendService)
    {
        _recommendService = recommendService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetTop([FromQuery] int month, [FromQuery] int year, [FromQuery] int limit = 10)
    {
        var result = await _recommendService.GetTopAsync(month, year, limit);
        return Ok(result);
    }

    [HttpGet("{comicId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetComicRecommends(long comicId, [FromQuery] int month, [FromQuery] int year)
    {
        var result = await _recommendService.GetByComicAndPeriodAsync(comicId, month, year);
        if (result == null)
        {
            return NotFound(new { message = "Chưa có dữ liệu đề cử" });
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.User)]
    public async Task<IActionResult> Recommend([FromBody] RecommendComicRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không xác định được người dùng" });
        }

        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        var result = await _recommendService.RecommendAsync(comicId, userId.Value);
        return Ok(result);
    }
}
