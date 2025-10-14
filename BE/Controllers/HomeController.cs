using TruyenCV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Helpers;
using Newtonsoft.Json.Linq;

namespace TruyenCV.Controllers
{
	[ApiController]
	public class HomeController : ControllerBase
	{
		[HttpGet("/ping")]
		public async Task<IActionResult> Index()
		{
			return Ok(new { message = "Pong" });
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
