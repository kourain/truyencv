using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTOs.Request;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class ComicChapterController : ControllerBase
{
	private readonly IComicChapterService _chapterService;
	private readonly IDistributedCache RedisCache;

	public ComicChapterController(IComicChapterService chapterService, IDistributedCache cache)
	{
		_chapterService = chapterService;
		RedisCache = cache;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(long id)
	{
		var chapter = await _chapterService.GetChapterByIdAsync(id);
		if (chapter == null)
			return NotFound(new { message = "Không tìm thấy chapter" });

		return Ok(chapter);
	}

	[HttpGet("comic/{comicId}/chapter/{chapter}")]
	public async Task<IActionResult> GetByComicAndChapter(long comicId, int chapter)
	{
		var chapterEntity = await _chapterService.GetChapterByComicIdAndChapterAsync(comicId, chapter);
		if (chapterEntity == null)
			return NotFound(new { message = "Không tìm thấy chapter" });

		return Ok(chapterEntity);
	}

	[HttpGet("comic/{comicId}")]
	public async Task<IActionResult> GetByComicId(long comicId)
	{
		var chapters = await _chapterService.GetChaptersByComicIdAsync(comicId);
		return Ok(chapters);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateComicChapterRequest request)
	{
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

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, [FromBody] UpdateComicChapterRequest request)
	{
		var requestId = request.id.ToSnowflakeId(nameof(request.id));
		if (id != requestId)
			return BadRequest(new { message = "ID không khớp" });

		try
		{
			var chapter = await _chapterService.UpdateChapterAsync(requestId, request);
			if (chapter == null)
				return NotFound(new { message = "Không tìm thấy chapter" });

			return Ok(chapter);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var result = await _chapterService.DeleteChapterAsync(id);
		if (!result)
			return NotFound(new { message = "Không tìm thấy chapter" });

		return Ok(new { message = "Xóa chapter thành công" });
	}
}
