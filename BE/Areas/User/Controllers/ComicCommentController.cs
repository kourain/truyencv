using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.User.Controllers;

[ApiController]
[Area("User")]
[Authorize(Roles = Roles.User)]
[Route("User/[controller]")]
public sealed class ComicCommentController : ControllerBase
{
    private readonly IComicCommentService _commentService;

    public ComicCommentController(IComicCommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateComicCommentRequest request)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Không thể xác định người dùng" });
        }

        // Override user_id from token
        request.user_id = userId.Value.ToString();

        try
        {
            var created = await _commentService.CreateCommentAsync(request);
            return CreatedAtAction(nameof(Create), new { id = created.id }, created);
        }
        catch (UserRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
