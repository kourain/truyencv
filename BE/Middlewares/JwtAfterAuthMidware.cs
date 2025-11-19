using TruyenCV.Services;
using System.Text.Json;
using TruyenCV.Helpers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
namespace TruyenCV.Middleware
{
    public class JwtAfterAuthMidware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAfterAuthMidware> _logger;
        public JwtAfterAuthMidware(RequestDelegate next, ILogger<JwtAfterAuthMidware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var _cache = provider.GetRequiredService<IDistributedCache>();
            var refreshTokenId = context.User.GetRefreshTokenId(); // ensure permissions are loaded
            if(await _cache.GetStringAsync($"JWT_Banned:{refreshTokenId}") != null)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Phiên đăng nhập đã bị thu hồi" }));
                return;
            }
            await _next(context);
        }
    }
}
