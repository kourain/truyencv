using System.Diagnostics;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Middleware
{
    public class E500Midware
    {
        private readonly RequestDelegate _next;
        public E500Midware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Microsoft.Extensions.Hosting.Environments.Production)
            {
                try
                {
                    await _next(context);
                }
                catch (UserRequestException urex)
                {
                    // Nếu là lỗi do người dùng yêu cầu sai, trả về mã lỗi tương ứng
                    context.Response.StatusCode = urex.StatusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync($@"{{ ""message"": ""{urex.Message}"" }}");
                }
                catch (Exception ex)
                {
                    // Log lỗi chi tiết
                    Serilog.Log.Error(ex, "Internal Server Error: {Message}", ex.Message);

                    // Trả về mã lỗi 500 cho client
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync($@"{{ ""message"": ""Internal Server Error"" }}");
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
