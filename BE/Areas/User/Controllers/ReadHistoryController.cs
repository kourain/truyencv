using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class ReadHistoryController : ControllerBase
{
    private readonly IUserComicReadHistoryService _readHistoryService;

    public ReadHistoryController(IUserComicReadHistoryService readHistoryService)
    {
        _readHistoryService = readHistoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistories([FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var histories = await _readHistoryService.GetReadHistoryByUserIdAsync(userId.Value, limit);
        return Ok(histories);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertUserComicReadHistoryRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        var chapterId = request.chapter_id.ToSnowflakeId(nameof(request.chapter_id));
        var history = await _readHistoryService.UpsertReadHistoryAsync(userId.Value, comicId, chapterId);
        return Ok(history);
    }

    [HttpDelete("{comicId}")]
    public async Task<IActionResult> Delete(long comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var removed = await _readHistoryService.RemoveReadHistoryAsync(userId.Value, comicId);
        if (!removed)
        {
            return NotFound(new { message = "Không tìm thấy lịch sử đọc" });
        }

        return Ok(new { message = "Xóa lịch sử đọc thành công" });
    }
}
