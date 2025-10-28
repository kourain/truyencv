using System.Diagnostics;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using TruyenCV.Services;
using System.Text.Json;
using TruyenCV.Helpers;

namespace TruyenCV.Middleware
{
    public class JWTMidware
    {
        private readonly RequestDelegate _next;
        public JWTMidware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context,IServiceProvider provider)
        {
            var _authService = provider.GetRequiredService<IAuthService>();
            if (!context.Request.Path.StartsWithSegments("/auth"))
            {
                var access_token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                context.Request.Cookies.TryGetValue("refresh_token", out var refresh_token);
                if (access_token.Length > 15 && !string.IsNullOrWhiteSpace(refresh_token))
                {
                    var result = await _authService.RefreshTokenAsync(refresh_token);
                    if (result == null)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Phiên không hợp lệ hoặc đã hết hạn" }));
                        return;
                    }

                    var (accessToken, refreshToken) = result.Value;
                    context.Request.Headers["Authorization"] = "Bearer " + accessToken;
                    context.Response.Headers["Authorization"] = "Bearer " + accessToken;
                    context.Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(JwtHelper.RefreshTokenExpiryDays)
                    });
                    context.Response.Cookies.Append("access_token", accessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(JwtHelper.AccessTokenExpiryMinutes)
                    });
                }
            }
            await _next(context);
        }
    }
}
