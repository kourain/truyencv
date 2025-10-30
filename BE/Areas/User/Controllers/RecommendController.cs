using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class RecommendController : ControllerBase
{
    private readonly IComicRecommendService _recommendService;

    public RecommendController(IComicRecommendService recommendService)
    {
        _recommendService = recommendService;
    }

    [HttpPost("{comicId}")]
    public async Task<IActionResult> Recommend(long comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var result = await _recommendService.RecommendAsync(comicId, userId.Value);
        return Ok(result);
    }

    [HttpGet("{comicId}/status")]
    public async Task<IActionResult> Status(long comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var hasRecommended = await _recommendService.HasUserRecommendedAsync(comicId, userId.Value);
        return Ok(new { comic_id = comicId, has_recommended = hasRecommended });
    }
}
