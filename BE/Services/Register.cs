using Microsoft.Extensions.DependencyInjection;

namespace TruyenCV;

/// <summary>
/// Extension method để đăng ký các service vào DI container
/// </summary>
public static class ServicesRegisterExtensions
{
	/// <summary>
	/// Đăng ký Services vào DI container
	/// </summary>
	/// <param name="Services">IServiceCollection</param>
	/// <returns>IServiceCollection</returns>
	public static IServiceCollection AddServices(this IServiceCollection Services)
	{
		Services.AddScoped<TruyenCV.Services.IAuthService, TruyenCV.Services.AuthService>();
		Services.AddScoped<TruyenCV.Services.IUserService, TruyenCV.Services.UserService>();
		Services.AddScoped<TruyenCV.Services.IUserHasRoleService, TruyenCV.Services.UserHasRoleService>();
		Services.AddScoped<TruyenCV.Services.IComicService, TruyenCV.Services.ComicService>();
		Services.AddScoped<TruyenCV.Services.IComicCategoryService, TruyenCV.Services.ComicCategoryService>();
		Services.AddScoped<TruyenCV.Services.IComicChapterService, TruyenCV.Services.ComicChapterService>();
		Services.AddScoped<TruyenCV.Services.IComicCommentService, TruyenCV.Services.ComicCommentService>();
		Services.AddScoped<TruyenCV.Services.IComicHaveCategoryService, TruyenCV.Services.ComicHaveCategoryService>();
		return Services;
	}
}