public static class MiddlewareRegisterExtensions
{
	/// <summary>
	/// Đăng ký Middleware vào DI container
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static WebApplication AddMiddlewares(this WebApplication app)
	{
		app.UseMiddleware<TruyenCV.Middleware.AreaMiddleware>();
		return app;
	}
}