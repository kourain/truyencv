using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class ComicRecommendController : ControllerBase
{
    private readonly IComicRecommendService _recommendService;

    public ComicRecommendController(IComicRecommendService recommendService)
    {
        _recommendService = recommendService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTop([FromQuery] int month, [FromQuery] int year, [FromQuery] int limit = 10)
    {
        var result = await _recommendService.GetTopAsync(month, year, limit);
        return Ok(result);
    }

    [HttpGet("comic/{comicId}")]
    public async Task<IActionResult> GetByComic(long comicId, [FromQuery] int limit = 12)
    {
        var result = await _recommendService.GetByComicAsync(comicId, limit);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComicRecommendRequest request)
    {
        var result = await _recommendService.CreateOrUpdateAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateComicRecommendRequest request)
    {
        var requestId = request.id.ToSnowflakeId(nameof(request.id));
        if (id != requestId)
        {
            return BadRequest(new { message = "ID không khớp" });
        }

        var updated = await _recommendService.UpdateAsync(requestId, request);
        if (updated == null)
        {
            return NotFound(new { message = "Không tìm thấy bản ghi" });
        }

        return Ok(updated);
    }

    [HttpGet("{comicId}/period")]
    public async Task<IActionResult> GetByPeriod(long comicId, [FromQuery] int month, [FromQuery] int year)
    {
        var result = await _recommendService.GetByComicAndPeriodAsync(comicId, month, year);
        if (result == null)
        {
            return NotFound(new { message = "Không có dữ liệu đề cử" });
        }

        return Ok(result);
    }
}
