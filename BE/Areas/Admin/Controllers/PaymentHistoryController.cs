using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public class PaymentHistoryController : ControllerBase
{
    private readonly IPaymentHistoryService _paymentHistoryService;

    public PaymentHistoryController(IPaymentHistoryService paymentHistoryService)
    {
        _paymentHistoryService = paymentHistoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int offset = 0, [FromQuery] int limit = 50)
    {
        var histories = await _paymentHistoryService.GetAsync(offset, limit);
        return Ok(histories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (!long.TryParse(id, out var historyId))
        {
            return BadRequest(new { message = "ID không hợp lệ" });
        }

        var history = await _paymentHistoryService.GetByIdAsync(historyId);
        if (history == null)
        {
            return NotFound(new { message = "Không tìm thấy giao dịch" });
        }

        return Ok(history);
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        if (!long.TryParse(userId, out var parsedUserId))
        {
            return BadRequest(new { message = "User ID không hợp lệ" });
        }

        var histories = await _paymentHistoryService.GetByUserIdAsync(parsedUserId);
        return Ok(histories);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentHistoryRequest request)
    {
        var created = await _paymentHistoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.id }, created);
    }
}
