using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTO.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class BookmarkController : ControllerBase
{
    private readonly IUserComicBookmarkService _bookmarkService;

    public BookmarkController(IUserComicBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBookmarks()
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var bookmarks = await _bookmarkService.GetBookmarksByUserIdAsync(userId.Value);
        return Ok(bookmarks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserComicBookmarkRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comicId = request.comic_id.ToSnowflakeId(nameof(request.comic_id));
        var bookmark = await _bookmarkService.CreateBookmarkAsync(userId.Value, comicId);
        return Ok(bookmark);
    }

    [HttpDelete("{comicId}")]
    public async Task<IActionResult> Delete(long comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var removed = await _bookmarkService.RemoveBookmarkAsync(userId.Value, comicId);
        if (!removed)
        {
            return NotFound(new { message = "Bookmark không tồn tại" });
        }

        return Ok(new { message = "Xóa bookmark thành công" });
    }

    [HttpGet("{comicId}/status")]
    public async Task<IActionResult> CheckStatus(long comicId)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var isBookmarked = await _bookmarkService.IsBookmarkedAsync(userId.Value, comicId);
        return Ok(new
        {
            comic_id = comicId,
            is_bookmarked = isBookmarked
        });
    }
}
