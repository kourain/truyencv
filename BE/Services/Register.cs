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
		Services.AddSingleton<TruyenCV.Services.ITextEmbeddingService, TruyenCV.Services.TextEmbeddingService>();
		Services.AddScoped<TruyenCV.Services.IAuthService, TruyenCV.Services.AuthService>();
		Services.AddScoped<TruyenCV.Services.IUserService, TruyenCV.Services.UserService>();
		Services.AddScoped<TruyenCV.Services.IUserHasRoleService, TruyenCV.Services.UserHasRoleService>();
		Services.AddScoped<TruyenCV.Services.IUserHasPermissionService, TruyenCV.Services.UserHasPermissionService>();
		Services.AddScoped<TruyenCV.Services.IComicService, TruyenCV.Services.ComicService>();
		Services.AddScoped<TruyenCV.Services.IComicCategoryService, TruyenCV.Services.ComicCategoryService>();
		Services.AddScoped<TruyenCV.Services.IComicChapterService, TruyenCV.Services.ComicChapterService>();
		Services.AddScoped<TruyenCV.Services.IComicCommentService, TruyenCV.Services.ComicCommentService>();
		Services.AddScoped<TruyenCV.Services.IComicHaveCategoryService, TruyenCV.Services.ComicHaveCategoryService>();
		Services.AddScoped<TruyenCV.Services.IUserComicBookmarkService, TruyenCV.Services.UserComicBookmarkService>();
		Services.AddScoped<TruyenCV.Services.IUserComicReadHistoryService, TruyenCV.Services.UserComicReadHistoryService>();
		Services.AddScoped<TruyenCV.Services.IPasswordResetService, TruyenCV.Services.PasswordResetService>();
		Services.AddScoped<TruyenCV.Services.IAdminDashboardService, TruyenCV.Services.AdminDashboardService>();
		Services.AddScoped<TruyenCV.Services.ISubscriptionService, TruyenCV.Services.SubscriptionService>();
		Services.AddScoped<TruyenCV.Services.IUserHasSubscriptionService, TruyenCV.Services.UserHasSubscriptionService>();
		Services.AddScoped<TruyenCV.Services.IPaymentHistoryService, TruyenCV.Services.PaymentHistoryService>();
		Services.AddScoped<TruyenCV.Services.IUserUseCoinHistoryService, TruyenCV.Services.UserUseCoinHistoryService>();
		Services.AddScoped<TruyenCV.Services.IUserUseKeyHistoryService, TruyenCV.Services.UserUseKeyHistoryService>();
		Services.AddScoped<TruyenCV.Services.IUserComicUnlockHistoryService, TruyenCV.Services.UserComicUnlockHistoryService>();
		Services.AddScoped<TruyenCV.Services.IComicRecommendService, TruyenCV.Services.ComicRecommendService>();
		Services.AddScoped<TruyenCV.Services.IComicReportService, TruyenCV.Services.ComicReportService>();
		Services.AddScoped<TruyenCV.Services.IComicReadingService, TruyenCV.Services.ComicReadingService>();
		Services.AddScoped<TruyenCV.Services.IChapterEnhancementService, TruyenCV.Services.ChapterEnhancementService>();
		Services.AddScoped<TruyenCV.Services.IFirebaseAuthService, TruyenCV.Services.FirebaseAuthService>();
		return Services;
	}
}