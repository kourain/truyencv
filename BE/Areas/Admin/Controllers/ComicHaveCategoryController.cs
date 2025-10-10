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
public sealed class ComicHaveCategoryController : ControllerBase
{
	private readonly IComicHaveCategoryService _comicHaveCategoryService;
	private readonly IDistributedCache RedisCache;

	public ComicHaveCategoryController(IComicHaveCategoryService comicHaveCategoryService, IDistributedCache cache)
	{
		_comicHaveCategoryService = comicHaveCategoryService;
		RedisCache = cache;
	}

	/// <summary>
	/// Lấy danh sách categories của một comic
	/// </summary>
	[HttpGet("comic/{comicId}/categories")]
	public async Task<IActionResult> GetCategoriesByComicId(ulong comicId)
	{
		var categories = await _comicHaveCategoryService.GetCategoriesByComicIdAsync(comicId);
		return Ok(categories);
	}

	/// <summary>
	/// Lấy danh sách comics của một category
	/// </summary>
	[HttpGet("category/{categoryId}/comics")]
	public async Task<IActionResult> GetComicsByCategoryId(ulong categoryId)
	{
		var comics = await _comicHaveCategoryService.GetComicsByCategoryIdAsync(categoryId);
		return Ok(comics);
	}

	/// <summary>
	/// Thêm comic vào category
	/// </summary>
	[HttpPost]
	public async Task<IActionResult> AddComicToCategory([FromBody] CreateComicHaveCategoryRequest request)
	{
		try
		{
			var result = await _comicHaveCategoryService.AddComicToCategoryAsync(request);
			return Ok(new { message = "Thêm comic vào category thành công" });
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}

	/// <summary>
	/// Xóa comic khỏi category
	/// </summary>
	[HttpDelete("comic/{comicId}/category/{categoryId}")]
	public async Task<IActionResult> RemoveComicFromCategory(ulong comicId, ulong categoryId)
	{
		var result = await _comicHaveCategoryService.RemoveComicFromCategoryAsync(comicId, categoryId);
		if (!result)
			return NotFound(new { message = "Không tìm thấy comic trong category này" });

		return Ok(new { message = "Xóa comic khỏi category thành công" });
	}
}
