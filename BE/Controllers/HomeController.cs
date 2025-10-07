using TruyenCV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
	}
}
