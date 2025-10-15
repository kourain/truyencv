using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TruyenCV.DTOs.Response;
using TruyenCV.Services;

namespace TruyenCV.Areas.Admin.Controllers;

[ApiController]
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
[Route("Admin/[controller]")]
public sealed class DashboardController : ControllerBase
{
	private readonly IAdminDashboardService _dashboardService;
	private readonly IDistributedCache _redisCache;

	public DashboardController(IAdminDashboardService dashboardService, IDistributedCache redisCache)
	{
		_dashboardService = dashboardService;
		_redisCache = redisCache;
	}

	/// <summary>
	/// Lấy dữ liệu tổng quan cho dashboard quản trị
	/// </summary>
	/// <param name="topComics">Số lượng truyện nổi bật trả về</param>
	/// <param name="recentUsers">Số lượng người dùng mới trả về</param>
	/// <param name="categoryLimit">Số lượng danh mục hiển thị</param>
	[HttpGet("overview")]
	public async Task<IActionResult> GetOverview(
		[FromQuery] int topComics = 5,
		[FromQuery] int recentUsers = 6,
		[FromQuery] int categoryLimit = 12)
	{
		var cacheKey = $"admin_dashboard:overview:{topComics}:{recentUsers}:{categoryLimit}";
		try
		{
			var cached = await _redisCache.GetStringAsync(cacheKey);
			if (!string.IsNullOrEmpty(cached))
			{
				var cachedResponse = JsonConvert.DeserializeObject<AdminDashboardOverviewResponse>(cached);
				if (cachedResponse != null)
				{
					return Ok(cachedResponse);
				}
			}
		}
		catch (Exception ex)
		{
			Serilog.Log.Error(ex, "Redis error when reading admin dashboard overview cache");
		}

		var overview = await _dashboardService.GetOverviewAsync(topComics, recentUsers, categoryLimit);

		try
		{
			var options = new DistributedCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
				SlidingExpiration = TimeSpan.FromMinutes(3)
			};
			await _redisCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(overview), options);
		}
		catch (Exception ex)
		{
			Serilog.Log.Error(ex, "Redis error when writing admin dashboard overview cache");
		}

		return Ok(overview);
	}
}
