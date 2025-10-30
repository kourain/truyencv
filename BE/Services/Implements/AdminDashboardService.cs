using TruyenCV.DTOs.Response;
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

        var metrics = new AdminDashboardMetricsResponse
        {
            total_comics = await _comicRepository.CountAsync(),
            continuing_comics = await _comicRepository.CountAsync(comic => comic.status == ComicStatus.Continuing),
            completed_comics = await _comicRepository.CountAsync(comic => comic.status == ComicStatus.Completed),
            total_users = await _userRepository.CountAsync(),
            new_users_7_days = await _userRepository.CountAsync(user => user.created_at >= sevenDaysAgo),
            categories_count = await _comicCategoryRepository.CountAsync(),
            total_chapters = (await _comicChapterRepository.CountAsync()).ToString(),
            total_comments = (await _comicCommentRepository.CountAsync()).ToString(),
            total_bookmarks = (await _comicRepository.SumBookmarkCountAsync()).ToString(),
            active_admins = await _userHasRoleRepository.CountAsync(role => role.role_name == Roles.Admin)
        };

        var top_comics = await _comicRepository.GetTopRatedAsync(topComicsLimit);
        var recent_users = await _userRepository.GetRecentUsersAsync(recentUsersLimit);
        var categories = await _comicCategoryRepository.GetLatestAsync(categoryLimitValue);
        var topComicResponses = top_comics.Select(comic => comic.ToRespDTO()).ToArray();
        var recentUserResponses = recent_users.Select(user => user.ToRespDTO()).ToArray();

        var categoryHighlights = new List<AdminDashboardCategorySummaryResponse>();
        foreach (var category in categories)
        {
            var comics = await _comicHaveCategoryRepository.GetComicsByCategoryIdAsync(category.id);
            var summary = new AdminDashboardCategorySummaryResponse
            {
                id = category._id,
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
