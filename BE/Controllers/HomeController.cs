using TruyenCV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Helpers;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace TruyenCV.Controllers
{
	[ApiController]
	public class HomeController : ControllerBase
	{
		private readonly AppDataContext _context;
		private readonly ILogger<HomeController> _logger;

		public HomeController(AppDataContext context, ILogger<HomeController> logger)
		{
			_context = context;
			_logger = logger;
		}

		[HttpGet("/ping")]
		public async Task<IActionResult> Index()
		{
			var timestamp = DateTime.UtcNow;
			var apiStatus = "healthy";
			var overallStatus = "healthy";

			var databaseStatus = "healthy";
			long databaseLatencyMs = 0;
			string? databaseError = null;
			var databaseStopwatch = Stopwatch.StartNew();
			try
			{
				var canConnect = await _context.Users.FirstOrDefaultAsync(m => m.id != SystemUser.id) != null;
				if (!canConnect)
				{
					databaseStatus = "unhealthy";
					overallStatus = "degraded";
					databaseError = "Database connection returned false.";
				}
			}
			catch (Exception exception)
			{
				databaseStatus = "unhealthy";
				overallStatus = "unhealthy";
				databaseError = exception.Message;
				_logger.LogError(exception, "Database connectivity check failed during ping.");
			}
			finally
			{
				if (databaseStatus != "unhealthy")
				{
					databaseStopwatch.Stop();
					databaseLatencyMs = databaseStopwatch.ElapsedMilliseconds;
				}
			}

			return Ok(new
			{
				message = overallStatus,
				api = new
				{
					status = apiStatus,
					timestamp
				},
				dependencies = new
				{
					database = new
					{
						status = databaseStatus,
						latency_ms = databaseLatencyMs,
						error = databaseError
					}
				}
			});
		}
		[HttpGet("/HEAD")]
		public async Task<IActionResult> Head()
		{
			return Ok(HttpContext.Request.Headers);
		}
		[HttpGet("/snowflake")]
		public IActionResult snowflake()
		{
			JObject rs = new JObject();
			for (int i = 0; i < 20; i++)
			{
				var id = SnowflakeIdGenerator.NextId();
				rs[i.ToString()] = id;
			}
			return Ok(rs.ToString(Newtonsoft.Json.Formatting.Indented));
		}
	}
}
