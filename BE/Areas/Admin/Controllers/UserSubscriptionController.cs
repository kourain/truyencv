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
public class UserSubscriptionController : ControllerBase
{
    private readonly IUserHasSubscriptionService _userHasSubscriptionService;

    public UserSubscriptionController(IUserHasSubscriptionService userHasSubscriptionService)
    {
        _userHasSubscriptionService = userHasSubscriptionService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!long.TryParse(id, out var recordId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var subscription = await _userHasSubscriptionService.GetByIdAsync(recordId);
        if (subscription == null)
        {
            return NotFound(new { message = "Không tìm thấy dữ liệu" });
        }

        return Ok(subscription);
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        if (!long.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new { message = "User ID không hợp lệ" });
        }

        var subscriptions = await _userHasSubscriptionService.GetByUserIdAsync(parsedUserId);
        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserSubscriptionRequest request)
    {
        var created = await _userHasSubscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserSubscriptionRequest request)
    {
        if (!long.TryParse(id, out var recordId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var updated = await _userHasSubscriptionService.UpdateAsync(recordId, request);
        if (updated == null)
        {
            return NotFound(new { message = "Không tìm thấy dữ liệu" });
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!long.TryParse(id, out var recordId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var deleted = await _userHasSubscriptionService.DeleteAsync(recordId);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy dữ liệu" });
        }

        return NoContent();
    }
}
