public static class MiddlewareRegisterExtensions
{
	/// <summary>
	/// Đăng ký Middleware vào DI container
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication AddMiddlewares(this WebApplication app)
	{
		app.UseMiddleware<TruyenCV.Middleware.E500Midware>();
		app.UseMiddleware<TruyenCV.Middleware.JwtBeforeMidware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TruyenCV.Middleware.JwtAfterAuthMidware>();
		return app;
	}
	/// <summary>
	/// Đăng ký Middleware vào DI container
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication AddMiddlewaresAfterRouting(this WebApplication app)
	{
		app.UseMiddleware<TruyenCV.Middleware.AreaMiddleware>();
		return app;
	}
	
}