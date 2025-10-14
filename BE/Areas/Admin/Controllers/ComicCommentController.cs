using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTO.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class ComicCommentController : ControllerBase
{
	private readonly IComicCommentService _commentService;
	private readonly IDistributedCache RedisCache;

	public ComicCommentController(IComicCommentService commentService, IDistributedCache cache)
	{
		_commentService = commentService;
		RedisCache = cache;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(long id)
	{
		var comment = await _commentService.GetCommentByIdAsync(id);
		if (comment == null)
			return NotFound(new { message = "Không tìm thấy comment" });

		return Ok(comment);
	}

	[HttpGet("comic/{comicId}")]
	public async Task<IActionResult> GetByComicId(long comicId)
	{
		var comments = await _commentService.GetCommentsByComicIdAsync(comicId);
		return Ok(comments);
	}

	[HttpGet("chapter/{chapterId}")]
	public async Task<IActionResult> GetByChapterId(long chapterId)
	{
		var comments = await _commentService.GetCommentsByChapterIdAsync(chapterId);
		return Ok(comments);
	}

	[HttpGet("user/{userId}")]
	public async Task<IActionResult> GetByUserId(long userId)
	{
		var comments = await _commentService.GetCommentsByUserIdAsync(userId);
		return Ok(comments);
	}

	[HttpGet("replies/{commentId}")]
	public async Task<IActionResult> GetReplies(long commentId)
	{
		var replies = await _commentService.GetRepliesAsync(commentId);
		return Ok(replies);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateComicCommentRequest request)
	{
		try
		{
			var comment = await _commentService.CreateCommentAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = comment.id }, comment);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, [FromBody] UpdateComicCommentRequest request)
	{
		var requestId = request.id.ToSnowflakeId(nameof(request.id));
		if (id != requestId)
			return BadRequest(new { message = "ID không khớp" });

		try
		{
			var comment = await _commentService.UpdateCommentAsync(requestId, request);
			if (comment == null)
				return NotFound(new { message = "Không tìm thấy comment" });

			return Ok(comment);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var result = await _commentService.DeleteCommentAsync(id);
		if (!result)
			return NotFound(new { message = "Không tìm thấy comment" });

		return Ok(new { message = "Xóa comment thành công" });
	}
}
