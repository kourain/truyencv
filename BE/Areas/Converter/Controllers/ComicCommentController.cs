using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class ComicCommentController : ControllerBase
{
    private readonly IComicCommentService _commentService;
    private readonly IComicService _comicService;

    public ComicCommentController(IComicCommentService commentService, IComicService comicService)
    {
        _commentService = commentService;
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

        var comments = await _commentService.GetCommentsByComicIdAsync(parsedComicId);
        return Ok(comments);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound(new { message = "Không tìm thấy bình luận" });
        }

        var parsedComicId = comment.comic_id.ToSnowflakeId();
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy bình luận" });
        }

        return Ok(comment);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        var comment = await _commentService.GetCommentByIdAsync(id);
        if (comment == null)
        {
            return NotFound(new { message = "Không tìm thấy bình luận" });
        }

        var parsedComicId = comment.comic_id.ToSnowflakeId();
        if (!await _comicService.IsComicOwnerAsync(parsedComicId, userId.Value))
        {
            return NotFound(new { message = "Không tìm thấy bình luận" });
        }

        var deleted = await _commentService.DeleteCommentAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Không tìm thấy bình luận" });
        }

        return Ok(new { message = "Xóa bình luận thành công" });
    }
}
