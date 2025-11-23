using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
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
    private readonly IChapterEnhancementService _chapterEnhancementService;
    public ComicController(IComicReadingService comicReadingService, IComicService comicService, IChapterEnhancementService chapterEnhancementService)
    {
        _comicReadingService = comicReadingService;
        _comicService = comicService;
        _chapterEnhancementService = chapterEnhancementService;
    }
    [AllowAnonymous]
    [HttpGet("seo/{slug}")]
    public async Task<IActionResult> GetComicSEOBySlug([FromRoute] string slug)
    {
        slug = slug.ToLower();
        var result = await _comicService.GetComicSEOBySlugAsync(slug);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        return Ok(result);
    }
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetComicDetailBySlug([FromRoute] string slug)
    {
        slug = slug.ToLower();
        var userId = User.GetUserId();
        var result = await _comicService.GetComicDetailBySlugAsync(slug, userId);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{slug}/embedded")]
    public async Task<IActionResult> GetComicsEmbeddedBySameUser([FromRoute] string slug)
    {
        var related = await _comicService.GetComicsByEmbeddedBySlugAsync(slug);
        return Ok(related);
    }

    [HttpGet("{slug}/chapters")]
    public async Task<IActionResult> GetChapterList(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest(new { message = "Slug truyện không hợp lệ" });
        }

        slug = slug.ToLower();
        var userId = User.GetUserId();
        var result = await _comicService.GetComicChaptersBySlugAsync(slug, userId);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy truyện" });
        }

        return Ok(result);
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
        slug = slug.ToLower();
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

    [HttpPost("{slug}/chapters/{chapterNumber:int}/convert-tv")]
    public async Task<IActionResult> ConvertChapterToVietnamese(string slug, int chapterNumber, [FromBody] ConvertChapterRequest request)
    {
        if (string.IsNullOrWhiteSpace(slug) || chapterNumber <= 0)
        {
            return BadRequest(new { message = "Thông tin chương không hợp lệ" });
        }

        if (request == null || string.IsNullOrWhiteSpace(request.content))
        {
            return BadRequest(new { message = "Nội dung chương không được để trống" });
        }

        var converted = await _chapterEnhancementService.ConvertToVietnameseAsync(request.content);
        return Ok(new { content = converted });
    }

    [HttpPost("{slug}/chapters/{chapterNumber:int}/tts")]
    public async Task<IActionResult> GenerateChapterAudio(string slug, int chapterNumber, [FromBody] ChapterTtsRequest request)
    {
        if (string.IsNullOrWhiteSpace(slug) || chapterNumber <= 0)
        {
            return BadRequest(new { message = "Thông tin chương không hợp lệ" });
        }

        if (request == null || string.IsNullOrWhiteSpace(request.content) || string.IsNullOrWhiteSpace(request.reference_audio))
        {
            return BadRequest(new { message = "Thiếu nội dung chương hoặc giọng đọc" });
        }

        var audio = await _chapterEnhancementService.GenerateTtsAsync(request.content, request.reference_audio);
        return File(audio.Data, audio.ContentType, audio.FileName);
    }

    [HttpGet("tts/voices")]
    public async Task<IActionResult> GetTtsVoices()
    {
        var voices = await _chapterEnhancementService.GetAvailableVoicesAsync();
        return Ok(voices);
    }
}
