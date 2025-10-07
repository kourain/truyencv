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
public sealed class ComicController : ControllerBase
{
	private readonly IComicService _comicService;
	private readonly IDistributedCache RedisCache;

	public ComicController(IComicService comicService, IDistributedCache cache)
	{
		_comicService = comicService;
		RedisCache = cache;
	}

	/// <summary>
	/// Lấy thông tin comic theo ID
	/// </summary>
	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(long id)
	{
		var comic = await _comicService.GetComicByIdAsync(id);
		if (comic == null)
			return NotFound(new { message = "Không tìm thấy comic" });
		
		return Ok(comic);
	}

	/// <summary>
	/// Lấy thông tin comic theo slug
	/// </summary>
	[HttpGet("slug/{slug}")]
	public async Task<IActionResult> GetBySlug(string slug)
	{
		var comic = await _comicService.GetComicBySlugAsync(slug);
		if (comic == null)
			return NotFound(new { message = "Không tìm thấy comic" });
		
		return Ok(comic);
	}

	/// <summary>
	/// Tìm kiếm comic
	/// </summary>
	[HttpGet("search")]
	public async Task<IActionResult> Search([FromQuery] string keyword)
	{
		var comics = await _comicService.SearchComicsAsync(keyword);
		return Ok(comics);
	}

	/// <summary>
	/// Lấy danh sách comic theo tác giả
	/// </summary>
	[HttpGet("author/{author}")]
	public async Task<IActionResult> GetByAuthor(string author)
	{
		var comics = await _comicService.GetComicsByAuthorAsync(author);
		return Ok(comics);
	}

	/// <summary>
	/// Lấy danh sách comic theo trạng thái
	/// </summary>
	[HttpGet("status/{status}")]
	public async Task<IActionResult> GetByStatus(ComicStatus status)
	{
		var comics = await _comicService.GetComicsByStatusAsync(status);
		return Ok(comics);
	}

	/// <summary>
	/// Lấy danh sách comic với phân trang
	/// </summary>
	[HttpGet]
	public async Task<IActionResult> GetAll([FromQuery] int offset = 0, [FromQuery] int limit = 10)
	{
		var comics = await _comicService.GetComicsAsync(offset, limit);
		return Ok(comics);
	}

	/// <summary>
	/// Tạo comic mới
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateComicRequest request)
	{
		try
		{
			var comic = await _comicService.CreateComicAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = comic.id }, comic);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Cập nhật thông tin comic
	/// </summary>
	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, [FromBody] UpdateComicRequest request)
	{
		if (id != request.id)
			return BadRequest(new { message = "ID không khớp" });

		try
		{
			var comic = await _comicService.UpdateComicAsync(id, request);
			if (comic == null)
				return NotFound(new { message = "Không tìm thấy comic" });
			
			return Ok(comic);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Xóa comic
	/// </summary>
	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var result = await _comicService.DeleteComicAsync(id);
		if (!result)
			return NotFound(new { message = "Không tìm thấy comic" });
		
		return Ok(new { message = "Xóa comic thành công" });
	}
}
