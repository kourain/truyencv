using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruyenCV.Services;

namespace TruyenCV.Areas.Converter.Controllers;

[ApiController]
[Area("Converter")]
[Authorize(Roles = Roles.Converter)]
[Route("Converter/[controller]")]
public sealed class ComicCategoryController : ControllerBase
{
	private readonly IComicCategoryService _categoryService;

	public ComicCategoryController(IComicCategoryService categoryService)
	{
		_categoryService = categoryService;
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
}
