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
public sealed class ComicCategoryController : ControllerBase
{
	private readonly IComicCategoryService _categoryService;
	private readonly IDistributedCache RedisCache;

	public ComicCategoryController(IComicCategoryService categoryService, IDistributedCache cache)
	{
		_categoryService = categoryService;
		RedisCache = cache;
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(long id)
	{
		var category = await _categoryService.GetCategoryByIdAsync(id);
		if (category == null)
			return NotFound(new { message = "Không tìm thấy thể loại" });
		
		return Ok(category);
	}

	[HttpGet("name/{name}")]
	public async Task<IActionResult> GetByName(string name)
	{
		var category = await _categoryService.GetCategoryByNameAsync(name);
		if (category == null)
			return NotFound(new { message = "Không tìm thấy thể loại" });
		
		return Ok(category);
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAll()
	{
		var categories = await _categoryService.GetAllCategoriesAsync();
		return Ok(categories);
	}

	[HttpGet]
	public async Task<IActionResult> GetPaged([FromQuery] int offset = 0, [FromQuery] int limit = 10)
	{
		var categories = await _categoryService.GetCategoriesAsync(offset, limit);
		return Ok(categories);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateComicCategoryRequest request)
	{
		try
		{
			var category = await _categoryService.CreateCategoryAsync(request);
			return CreatedAtAction(nameof(GetById), new { id = category.id }, category);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(long id, [FromBody] UpdateComicCategoryRequest request)
	{
		var requestId = request.id.ToSnowflakeId(nameof(request.id));
		if (id != requestId)
			return BadRequest(new { message = "ID không khớp" });

		try
		{
			var category = await _categoryService.UpdateCategoryAsync(requestId, request);
			if (category == null)
				return NotFound(new { message = "Không tìm thấy thể loại" });
			
			return Ok(category);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(long id)
	{
		var result = await _categoryService.DeleteCategoryAsync(id);
		if (!result)
			return NotFound(new { message = "Không tìm thấy thể loại" });
		
		return Ok(new { message = "Xóa thể loại thành công" });
	}
}
