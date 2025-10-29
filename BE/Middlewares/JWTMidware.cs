using System.Diagnostics;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Services;
using System.Text.Json;
using TruyenCV.Helpers;
using Serilog;
using System.Text.RegularExpressions;
namespace TruyenCV.Middleware
{
    public class JWTMidware
    {
        private readonly RequestDelegate _next;
        public JWTMidware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IServiceProvider provider)
        {
            if (!Regex.IsMatch(context.Request.Path, "^/auth/(refresh-token|login)", RegexOptions.IgnoreCase))
            {
                var _authService = provider.GetRequiredService<IAuthService>();
                var access_token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var refresh_token = context.Request.Headers["X-Refresh-Token"].ToString();
                // if access_token not right :???
                if (access_token.Length < 15 && !string.IsNullOrWhiteSpace(refresh_token))
                {
                    if (await _authService.RefreshTokenAsync(refresh_token) is (string, string) auth)
                    {
                        var (accessToken, refreshToken) = auth;
                        context.Request.Headers["Authorization"] = "Bearer " + accessToken;
                        context.Response.Headers["X-Refresh-Token"] = refreshToken;
                        context.Response.Headers["X-Access-Token"] = accessToken;
                        context.Response.Headers["X-Access-Token-Expiry"] = JwtHelper.AccessTokenExpiryMinutes.ToString();
                        context.Response.Headers["X-Refresh-Token-Expiry"] = JwtHelper.RefreshTokenExpiryDays.ToString();
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
            await _next(context);
        }
    }
}
