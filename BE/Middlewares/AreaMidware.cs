using System.Diagnostics;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Middleware
{
	public class AreaMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly AppDataContext _db;
		public AreaMiddleware(RequestDelegate next, IServiceProvider provider)
		{
			_next = next;
			_db = provider.CreateScope().ServiceProvider.GetRequiredService<AppDataContext>();
		}
		public async Task InvokeAsync(HttpContext context)
		{
			foreach (var route in context.Request.RouteValues)
			{
				Console.WriteLine($"{route.Key}: {route.Value}");
			}
			//EG: 
			// area: V2
			// action: GetById
			// controller: User
			// id: 1
			switch (context.Request.RouteValues["area"])
			{
				case "V1":
					// Xử lý cho area V1
					break;

				case "V2":
					// Xử lý cho area V1
					break;

				default:
					context.Response.StatusCode = 404;
					break;
			}
			await _next(context);
		}
	}
}
