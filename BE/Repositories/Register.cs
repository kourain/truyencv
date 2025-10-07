public static class RepositoriesRegisterExtensions
{
	/// <summary>
	/// Đăng ký Repositories vào DI container
	/// </summary>
	/// <param name="Services"></param>
	/// <returns></returns>
	public static IServiceCollection AddRepositories(this IServiceCollection Services)
	{
		Services.AddScoped<TruyenCV.Repositories.IUserRepository, TruyenCV.Repositories.UserRepository>();
		Services.AddScoped<TruyenCV.Repositories.IRefreshTokenRepository, TruyenCV.Repositories.RefreshTokenRepository>();
		return Services;
	}
}