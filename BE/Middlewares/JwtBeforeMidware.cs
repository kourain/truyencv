using TruyenCV.Services;
using System.Text.Json;
using TruyenCV.Helpers;
using System.Text.RegularExpressions;
using TruyenCV.Models;
namespace TruyenCV.Middleware
{
    public class JwtBeforeMidware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtBeforeMidware> _logger;
        public JwtBeforeMidware(RequestDelegate next, ILogger<JwtBeforeMidware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var access_token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var refresh_token = context.Request.Headers["X-Refresh-Token"].ToString();
            if (string.IsNullOrWhiteSpace(refresh_token))
            {
                refresh_token = "3kVGEq5i048o+TiRYe31aB83QsL99HAdAAQp/zZE4jc="; // default refresh token for public routes
            }
            // if access_token not right :???
            if (access_token.Length < 15 && !string.IsNullOrWhiteSpace(refresh_token))
            {
                var _authService = provider.GetRequiredService<IAuthService>();
                if (await _authService.RefreshTokenAsync(refresh_token) is (string, string) auth)
                {
                    var (accessToken, refreshToken) = auth;
                    context.Request.Headers.Authorization = $"Bearer {accessToken}";
                    access_token = accessToken;
                }
                else
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Phiên không hợp lệ hoặc đã hết hạn" }));
                    return;
                }
            }
            context.Response.Headers["X-Refresh-Token"] = refresh_token;
            context.Response.Headers["X-Access-Token"] = access_token;
            context.Response.Headers["X-Access-Token-Expiry"] = JwtHelper.AccessTokenExpiryMinutes.ToString();
            context.Response.Headers["X-Refresh-Token-Expiry"] = JwtHelper.RefreshTokenExpiryDays.ToString();
            var _database = provider.GetRequiredService<AppDataContext>();
            _database.ChangeTracker.Clear();
            await _next(context);
        }
    }
}
