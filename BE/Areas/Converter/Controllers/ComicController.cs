using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class ComicController : ControllerBase
{
    private readonly IComicService _comicService;

    public ComicController(IComicService comicService)
    {
        _comicService = comicService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyComics([FromQuery] int offset = 0, [FromQuery] int limit = 20)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comics = await _comicService.GetComicsByEmbeddedByAsync(userId.Value, offset, limit);
        return Ok(comics);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comic = await _comicService.GetComicOwnedByAsync(id, userId.Value);
        if (comic == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        return Ok(comic);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComicRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        try
        {
            var comic = await _comicService.CreateComicAsync(request, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = comic.id }, comic);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateComicRequest request)
    {
        var requestId = request.id.ToSnowflakeId(nameof(request.id));
        if (requestId != id)
        {
            return BadRequest(new { message = "ID không khớp" });
        }

        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var ownsComic = await _comicService.IsComicOwnerAsync(id, userId.Value);
        if (!ownsComic)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        try
        {
            var comic = await _comicService.UpdateComicAsync(id, request);
            if (comic == null)
            {
                return NotFound(new { message = "Không tìm thấy truyện" });
            }

            return Ok(comic);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var ownsComic = await _comicService.IsComicOwnerAsync(id, userId.Value);
        if (!ownsComic)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        var deleted = await _comicService.DeleteComicAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        return Ok(new { message = "Xóa truyện thành công" });
    }
}
