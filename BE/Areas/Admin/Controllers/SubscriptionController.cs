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
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int offset = 0, [FromQuery] int limit = 20)
    {
        var subscriptions = await _subscriptionService.GetAsync(offset, limit);
        return Ok(subscriptions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!long.TryParse(id, out var subscriptionId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var subscription = await _subscriptionService.GetByIdAsync(subscriptionId);
        if (subscription == null)
        {
            return NotFound(new { message = "Không tìm thấy gói thuê bao" });
        }

        return Ok(subscription);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionRequest request)
    {
        var created = await _subscriptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSubscriptionRequest request)
    {
        if (!long.TryParse(id, out var subscriptionId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var updated = await _subscriptionService.UpdateAsync(subscriptionId, request);
        if (updated == null)
        {
            return NotFound(new { message = "Không tìm thấy gói thuê bao" });
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (!long.TryParse(id, out var subscriptionId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var deleted = await _subscriptionService.DeleteAsync(subscriptionId);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy gói thuê bao" });
        }

        return NoContent();
    }
}
