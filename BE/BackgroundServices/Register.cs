public static class BackgroundServicesRegisterExtensions
{
	/// <summary>
	/// Đăng ký BackgroundServices vào DI container
	/// </summary>
	/// <param name="Services"></param>
	/// <returns></returns>
	public static IServiceCollection AddBackgroundServices(this IServiceCollection Services)
	{
        Services.AddHostedService<TruyenCV.BackgroundServices.TemplateBackgroundService>();
        Services.AddHostedService<TruyenCV.BackgroundServices.ComicEmbedBackgroundService>();
		return Services;
	}
}