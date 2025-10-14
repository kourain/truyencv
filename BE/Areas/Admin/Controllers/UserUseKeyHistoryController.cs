using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Const;
using TruyenCV.DTO.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public class UserUseKeyHistoryController : ControllerBase
{
    private readonly IUserUseKeyHistoryService _userUseKeyHistoryService;

    public UserUseKeyHistoryController(IUserUseKeyHistoryService userUseKeyHistoryService)
    {
        _userUseKeyHistoryService = userUseKeyHistoryService;
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        if (!long.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new { message = "User ID không hợp lệ" });
        }

        var histories = await _userUseKeyHistoryService.GetByUserIdAsync(parsedUserId);
        return Ok(histories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserUseKeyHistoryRequest request)
    {
        var created = await _userUseKeyHistoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUser), new { userId = created.user_id }, created);
    }
}
