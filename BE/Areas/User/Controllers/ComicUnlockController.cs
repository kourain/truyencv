using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class ComicUnlockController : ControllerBase
{
    private readonly IUserComicUnlockHistoryService _unlockHistoryService;

    public ComicUnlockController(IUserComicUnlockHistoryService unlockHistoryService)
    {
        _unlockHistoryService = unlockHistoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var histories = await _unlockHistoryService.GetByUserIdAsync(userId.Value);
        return Ok(histories);
    }

    [HttpGet("chapter/{chapterId}")]
    public async Task<IActionResult> CheckChapter(string chapterId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        if (!long.TryParse(chapterId, out var parsedChapterId))
        {
            return BadRequest(new { message = "Chapter ID không hợp lệ" });
        }

        var unlocked = await _unlockHistoryService.HasUnlockedChapterAsync(userId.Value, parsedChapterId);
        return Ok(new { unlocked });
    }

    [HttpPost]
    public async Task<IActionResult> Unlock([FromBody] UnlockComicChapterRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var result = await _unlockHistoryService.UnlockChapterAsync(userId.Value, request);
        return Ok(result);
    }
}
