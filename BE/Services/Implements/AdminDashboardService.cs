using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TruyenCV;
using TruyenCV.DTO.Response;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public sealed class AdminDashboardService : IAdminDashboardService
{
	private readonly IComicRepository _comicRepository;
	private readonly IUserRepository _userRepository;
	private readonly IComicCategoryRepository _comicCategoryRepository;
	private readonly IComicChapterRepository _comicChapterRepository;
	private readonly IComicCommentRepository _comicCommentRepository;
	private readonly IUserHasRoleRepository _userHasRoleRepository;
	private readonly IComicHaveCategoryRepository _comicHaveCategoryRepository;

	public AdminDashboardService(
		IComicRepository comicRepository,
		IUserRepository userRepository,
		IComicCategoryRepository comicCategoryRepository,
		IComicChapterRepository comicChapterRepository,
		IComicCommentRepository comicCommentRepository,
		IUserHasRoleRepository userHasRoleRepository,
		IComicHaveCategoryRepository comicHaveCategoryRepository)
	{
		_comicRepository = comicRepository;
		_userRepository = userRepository;
		_comicCategoryRepository = comicCategoryRepository;
		_comicChapterRepository = comicChapterRepository;
		_comicCommentRepository = comicCommentRepository;
		_userHasRoleRepository = userHasRoleRepository;
		_comicHaveCategoryRepository = comicHaveCategoryRepository;
	}

	public async Task<AdminDashboardOverviewResponse> GetOverviewAsync(int topComics = 5, int recentUsers = 6, int categoryLimit = 12)
	{
		var topComicsLimit = Math.Clamp(topComics, 1, 50);
		var recentUsersLimit = Math.Clamp(recentUsers, 1, 50);
		var categoryLimitValue = Math.Clamp(categoryLimit, 1, 50);

		var now = DateTime.UtcNow;
		var sevenDaysAgo = now.AddDays(-7);

		var totalComics = await _comicRepository.CountAsync();
		var continuingComics = await _comicRepository.CountAsync(comic => comic.status == ComicStatus.Continuing);
		var completedComics = await _comicRepository.CountAsync(comic => comic.status == ComicStatus.Completed);
		var totalUsers = await _userRepository.CountAsync();
		var newUsers = await _userRepository.CountAsync(user => user.created_at >= sevenDaysAgo);
		var categoriesCount = await _comicCategoryRepository.CountAsync();
		var totalChapters = await _comicChapterRepository.CountAsync();
		var totalComments = await _comicCommentRepository.CountAsync();
		var totalBookmarks = await _comicRepository.SumBookmarkCountAsync();
		var topComicsResult = await _comicRepository.GetTopRatedAsync(topComicsLimit);
		var recentUsersResult = await _userRepository.GetRecentUsersAsync(recentUsersLimit);
		var categories = await _comicCategoryRepository.GetLatestAsync(categoryLimitValue);
		var activeAdmins = await _userHasRoleRepository.CountAsync(role => role.role_name == Roles.Admin);

		// await Task.WhenAll(
		// 	totalComicsTask,
		// 	continuingComicsTask,
		// 	completedComicsTask,
		// 	averageRatingTask,
		// 	totalUsersTask,
		// 	newUsersTask,
		// 	categoriesCountTask,
		// 	totalChaptersTask,
		// 	totalCommentsTask,
		// 	totalBookmarksTask,
		// 	topComicsTask,
		// 	recentUsersTask,
		// 	categoriesTask,
		// 	activeAdminsTask
		// );

		var metrics = new AdminDashboardMetricsResponse
		{
			total_comics = totalComics,
			continuing_comics = continuingComics,
			completed_comics = completedComics,
			total_users = totalUsers,
			new_users_7_days = newUsers,
			categories_count = categoriesCount,
			total_chapters = totalChapters,
			total_comments = totalComments,
			total_bookmarks = totalBookmarks,
			active_admins = activeAdmins
		};

		var topComicResponses = topComicsResult.Select(comic => comic.ToRespDTO()).ToArray();
		var recentUserResponses = recentUsersResult.Select(user => user.ToRespDTO()).ToArray();

		var categoryHighlights = new List<AdminDashboardCategorySummaryResponse>();
		foreach (var category in categories)
		{
			var comics = await _comicHaveCategoryRepository.GetComicsByCategoryIdAsync(category.id);
			var summary = new AdminDashboardCategorySummaryResponse
			{
				id = category.id,
				name = category.name,
				created_at = category.created_at,
				updated_at = category.updated_at,
				comics_count = comics.Count()
			};
			categoryHighlights.Add(summary);
		}

		return new AdminDashboardOverviewResponse
		{
			metrics = metrics,
			top_comics = topComicResponses,
			recent_users = recentUserResponses,
			category_highlights = categoryHighlights,
			generated_at = now,
			is_mock = false
		};
	}
}
