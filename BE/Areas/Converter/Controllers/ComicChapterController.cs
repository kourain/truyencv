using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class ComicChapterController : ControllerBase
{
    private readonly IComicChapterService _chapterService;
    private readonly IComicService _comicService;

    public ComicChapterController(IComicChapterService chapterService, IComicService comicService)
    {
        _chapterService = chapterService;
        _comicService = comicService;
    }

    [HttpGet("comic/{comicId}")]
    public async Task<IActionResult> GetByComic(string comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var parsedComicId = comicId.ToSnowflakeId(nameof(comicId));
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        var chapters = await _chapterService.GetChaptersByComicIdAsync(parsedComicId);
        return Ok(chapters);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var chapter = await _chapterService.GetChapterByIdAsync(id);
        if (chapter == null)
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        var parsedComicId = chapter.comic_id.ToSnowflakeId();
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        return Ok(chapter);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComicChapterRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var parsedComicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        try
        {
            var chapter = await _chapterService.CreateChapterAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = chapter.id }, chapter);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateComicChapterRequest request)
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

        var parsedComicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        try
        {
            var chapter = await _chapterService.UpdateChapterAsync(id, request);
            if (chapter == null)
            {
                return NotFound(new { message = "Không tìm thấy chương" });
            }

            return Ok(chapter);
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

        var chapter = await _chapterService.GetChapterByIdAsync(id);
        if (chapter == null)
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        var parsedComicId = chapter.comic_id.ToSnowflakeId();
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        var deleted = await _chapterService.DeleteChapterAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        return Ok(new { message = "Xóa chương thành công" });
    }
}
