using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[AllowAnonymous]
[Route("User/[controller]")]
public sealed class ComicController : ControllerBase
{
    private readonly IComicReadingService _comicReadingService;

    public ComicController(IComicReadingService comicReadingService)
    {
        _comicReadingService = comicReadingService;
    }

    [HttpGet("{slug}/chapters/{chapterNumber:int}")]
    public async Task<IActionResult> GetChapter(string slug, int chapterNumber)
    {
        var result = await _comicReadingService.GetChapterAsync(slug, chapterNumber);
        if (result == null)
        {
            return NotFound(new { message = "Không tìm thấy chương" });
        }

        return Ok(result);
    }
}
