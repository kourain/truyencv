using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class UserComicUnlockHistoryController : ControllerBase
{
    private readonly IUserComicUnlockHistoryService _unlockHistoryService;

    public UserComicUnlockHistoryController(IUserComicUnlockHistoryService unlockHistoryService)
    {
        _unlockHistoryService = unlockHistoryService;
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        if (!long.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new { message = "User ID không hợp lệ" });
        }

        var histories = await _unlockHistoryService.GetByUserIdAsync(parsedUserId);
        return Ok(histories);
    }

    [HttpGet("by-comic/{comicId}")]
    public async Task<IActionResult> GetByComic(string comicId)
    {
        if (!long.TryParse(comicId, out var parsedComicId))
        {
            return BadRequest(new { message = "Comic ID không hợp lệ" });
        }

        var histories = await _unlockHistoryService.GetByComicIdAsync(parsedComicId);
        return Ok(histories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserComicUnlockHistoryRequest request)
    {
        var created = await _unlockHistoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUser), new { userId = created.user_id }, created);
    }
}
