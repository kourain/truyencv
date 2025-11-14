using System.Diagnostics;
using Microsoft.AspNetCore.Http;
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

			var hasArea = context.Request.RouteValues.TryGetValue("area", out var areaValue);
			var area = hasArea ? areaValue?.ToString() : null;

			switch (area)
			{
				case null:
					await _next(context);
					return;
				case "Admin":
					await _next(context);
					return;
				case "User":
					await _next(context);
					return;
                case "Converter":
                    await _next(context);
                    return;
				default:
					context.Response.StatusCode = StatusCodes.Status404NotFound;
					return;
			}
		}
	}
}
