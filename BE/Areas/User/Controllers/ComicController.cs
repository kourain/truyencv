using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class ComicController : ControllerBase
{
    private readonly IComicReadingService _comicReadingService;
    private readonly IComicService _comicService;
    public ComicController(IComicReadingService comicReadingService, IComicService comicService)
    {
        _comicReadingService = comicReadingService;
        _comicService = comicService;
    }
    [AllowAnonymous]
    [HttpGet("/seo/{slug}")]
    public async Task<IActionResult> GetComicSEOBySlug([FromRoute] string slug)
    {
        var result = await _comicService.GetComicSEOBySlugAsync(slug);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        return Ok(result);
    }
    [HttpGet("/{slug}")]
    public async Task<IActionResult> GetComicDetailBySlug([FromRoute] string slug)
    {
        var result = await _comicService.GetComicDetailBySlugAsync(slug);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("/{slug}/embedded")]
    public async Task<IActionResult> GetComicsEmbeddedBySameUser([FromRoute] string slug)
    {
        var related = await _comicService.GetComicsByEmbeddedBySlugAsync(slug);
        return Ok(related);
    }
    [HttpGet("{slug}/chapters/{chapterNumber:int}")]
    public async Task<IActionResult> GetChapter(string slug, int chapterNumber)
    {
        if(string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest(new { message = "Slug truyện không hợp lệ" });
        }
        if(chapterNumber <= 0)
        {
            return BadRequest(new { message = "Số chương không hợp lệ" });
        }
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }
        // lấy chương
        var result = await _comicReadingService.GetChapterAsync(slug, chapterNumber, userId.Value);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }
        // ghi lịch sử
        await _comicReadingService.RecordChapterReadAsync(result.comic_id.ToSnowflakeId(), chapterNumber, userId.Value);
        return Ok(result);
    }
}
