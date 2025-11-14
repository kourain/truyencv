using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class CoinHistoryController : ControllerBase
{
    private readonly IUserUseCoinHistoryService _coinHistoryService;

    public CoinHistoryController(IUserUseCoinHistoryService coinHistoryService)
    {
        _coinHistoryService = coinHistoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var histories = await _coinHistoryService.GetByUserIdAsync(userId.Value);
        return Ok(histories);
    }
}
