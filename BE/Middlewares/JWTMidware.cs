using TruyenCV.Services;
using System.Text.Json;
using TruyenCV.Helpers;
using System.Text.RegularExpressions;
namespace TruyenCV.Middleware
{
    public class JWTMidware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JWTMidware> _logger;
        public JWTMidware(RequestDelegate next, ILogger<JWTMidware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            var access_token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var refresh_token = context.Request.Headers["X-Refresh-Token"].ToString();
            if (!Regex.IsMatch(context.Request.Path, "^/auth/(refresh-token|login)", RegexOptions.IgnoreCase))
            {
                var _authService = provider.GetRequiredService<IAuthService>();
                // if access_token not right :???
                if (access_token.Length < 15 && !string.IsNullOrWhiteSpace(refresh_token))
                {
                    if (await _authService.RefreshTokenAsync(refresh_token) is (string, string) auth)
                    {
                        var (accessToken, refreshToken) = auth;
                        context.Request.Headers.Authorization = "Bearer " + accessToken;
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
            }
            context.Response.Headers["X-Refresh-Token"] = refresh_token;
            context.Response.Headers["X-Access-Token"] = access_token;
            context.Response.Headers["X-Access-Token-Expiry"] = JwtHelper.AccessTokenExpiryMinutes.ToString();
            context.Response.Headers["X-Refresh-Token-Expiry"] = JwtHelper.RefreshTokenExpiryDays.ToString();
            await _next(context);
        }
    }
}
