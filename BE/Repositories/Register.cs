using Microsoft.Extensions.DependencyInjection;

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
		Services.AddScoped<TruyenCV.Repositories.IUserHasRoleRepository, TruyenCV.Repositories.UserHasRoleRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserHasPermissionRepository, TruyenCV.Repositories.UserHasPermissionRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicRepository, TruyenCV.Repositories.ComicRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicCategoryRepository, TruyenCV.Repositories.ComicCategoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicChapterRepository, TruyenCV.Repositories.ComicChapterRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicCommentRepository, TruyenCV.Repositories.ComicCommentRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicHaveCategoryRepository, TruyenCV.Repositories.ComicHaveCategoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserComicBookmarkRepository, TruyenCV.Repositories.UserComicBookmarkRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserComicReadHistoryRepository, TruyenCV.Repositories.UserComicReadHistoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.ISubscriptionRepository, TruyenCV.Repositories.SubscriptionRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserHasSubscriptionRepository, TruyenCV.Repositories.UserHasSubscriptionRepository>();
		Services.AddScoped<TruyenCV.Repositories.IPaymentHistoryRepository, TruyenCV.Repositories.PaymentHistoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserCoinHistoryRepository, TruyenCV.Repositories.UserCoinHistoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserUseKeyHistoryRepository, TruyenCV.Repositories.UserUseKeyHistoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IUserComicUnlockHistoryRepository, TruyenCV.Repositories.UserComicUnlockHistoryRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicRecommendRepository, TruyenCV.Repositories.ComicRecommendRepository>();
		Services.AddScoped<TruyenCV.Repositories.IComicReportRepository, TruyenCV.Repositories.ComicReportRepository>();
		return Services;
	}
}