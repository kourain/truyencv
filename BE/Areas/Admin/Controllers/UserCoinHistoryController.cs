using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public class UserUseCoinHistoryController : ControllerBase
{
    private readonly IUserUseCoinHistoryService _UserUseCoinHistoryService;

    public UserUseCoinHistoryController(IUserUseCoinHistoryService UserUseCoinHistoryService)
    {
        _UserUseCoinHistoryService = UserUseCoinHistoryService;
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        if (!long.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new { message = "User ID không hợp lệ" });
        }

        var histories = await _UserUseCoinHistoryService.GetByUserIdAsync(parsedUserId);
        return Ok(histories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserUseCoinHistoryRequest request)
    {
        var created = await _UserUseCoinHistoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUser), new { userId = created.user_id }, created);
    }
}
