using System;
using System.Collections.Generic;
using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Pgvector;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của Comic Service
/// </summary>
public class ComicService : IComicService
{
    private readonly IComicRepository _comicRepository;
    private readonly IDistributedCache _redisCache;
    private readonly ITextEmbeddingService _embeddingService;
    private readonly IUserComicReadHistoryRepository _readHistoryRepository;
    private readonly IComicChapterRepository _comicChapterRepository;
    private readonly IComicRecommendRepository _comicRecommendRepository;
    private readonly IComicCommentRepository _comicCommentRepository;
    private readonly IComicHaveCategoryRepository _comicHaveCategoryRepository;
    private readonly IComicCategoryRepository _comicCategoryRepository;
    private readonly IUserComicUnlockHistoryRepository _comicUnlockHistoryRepository;

    public ComicService(
        IComicRepository comicRepository,
        IDistributedCache redisCache,
        ITextEmbeddingService embeddingService,
        IUserComicReadHistoryRepository readHistoryRepository,
        IComicChapterRepository comicChapterRepository,
        IComicRecommendRepository comicRecommendRepository,
        IComicCommentRepository comicCommentRepository,
        IComicHaveCategoryRepository comicHaveCategoryRepository,
        IComicCategoryRepository comicCategoryRepository,
        IUserComicUnlockHistoryRepository comicUnlockHistoryRepository)
    {
        _comicRepository = comicRepository;
        _redisCache = redisCache;
        _embeddingService = embeddingService;
        _readHistoryRepository = readHistoryRepository;
        _comicChapterRepository = comicChapterRepository;
        _comicRecommendRepository = comicRecommendRepository;
        _comicCommentRepository = comicCommentRepository;
        _comicHaveCategoryRepository = comicHaveCategoryRepository;
        _comicCategoryRepository = comicCategoryRepository;
        _comicUnlockHistoryRepository = comicUnlockHistoryRepository;
    }

    public async Task<ComicResponse?> GetComicByIdAsync(long id)
    {
        var comic = await _comicRepository.GetByIdAsync(id);
        return comic?.ToRespDTO();
    }

    public async Task<ComicResponse?> GetComicBySlugAsync(string slug)
    {
        slug = slug.ToLower();
        var comic = await _comicRepository.GetBySlugAsync(slug);
        if (comic?.deleted_at != null)
        {
            return null;
        }
        return comic?.ToRespDTO();
    }

    public async Task<ComicSeoResponse?> GetComicSEOBySlugAsync(string slug)
    {
        var comic = await _comicRepository.GetBySlugAsync(slug);
        if (comic == null || comic.deleted_at != null)
        {
            return null;
        }

        var keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(comic.name))
        {
            keywords.Add(comic.name);
            foreach (var part in comic.name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                keywords.Add(part);
            }
        }

        if (!string.IsNullOrWhiteSpace(comic.author))
        {
            keywords.Add(comic.author);
        }

        if (!string.IsNullOrWhiteSpace(comic.slug))
        {
            keywords.Add(comic.slug.Replace('-', ' '));
        }

        if (comic.ComicHaveCategories is { Count: > 0 })
        {
            foreach (var relation in comic.ComicHaveCategories)
            {
                if (!string.IsNullOrWhiteSpace(relation?.ComicCategory?.name))
                {
                    keywords.Add(relation.ComicCategory.name);
                }
            }
        }

        string description = comic.description;
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 180)
        {
            description = description[..180] + "...";
        }

        var image = comic.cover_url
            ?? comic.banner_url
            ?? comic.embedded_from_url
            ?? string.Empty;

        return new ComicSeoResponse
        {
            title = comic.name,
            description = description,
            keywords = keywords.Where(k => !string.IsNullOrWhiteSpace(k)).ToArray(),
            image = image
        };
    }

    public async Task<ComicDetailResponse?> GetComicDetailBySlugAsync(string slug, long? userId = null)
    {
        var comic = await _comicRepository.GetBySlugAsync(slug);
        if (comic == null || comic.deleted_at != null)
        {
            return null;
        }

        var chaptersTask = await _comicChapterRepository.GetByComicIdAsync(comic.id);
        var commentsTask = await _comicCommentRepository.GetByComicIdAsync(comic.id);
        var recommendationsTask = await _comicRecommendRepository.GetByComicAsync(comic.id, 12);
        var relatedTask = await _comicRepository.GetByAuthorAsync(comic.author);
        var chapterList = chaptersTask
            .Where(chapter => chapter.deleted_at == null)
            .OrderByDescending(chapter => chapter.chapter)
            .ThenByDescending(chapter => chapter.id)
            .ToList();

        var latestChapters = chapterList
            .Take(8)
            .Select(chapter => new ComicDetailChapterResponse
            {
                id = chapter._id,
                number = chapter.chapter,
                title = BuildChapterTitle(chapter.chapter),
                released_at = chapter.updated_at
            })
            .ToList();

        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        var weeklyChapterCount = chapterList.Count(chapter => chapter.updated_at >= sevenDaysAgo);

        var recommendEntries = recommendationsTask.ToList();
        var currentUtc = DateTime.UtcNow;
        var weeklyRecommendations = (int)Math.Clamp(
            recommendEntries
                .Where(entry => entry.year == currentUtc.Year && entry.month == currentUtc.Month)
                .Sum(entry => entry.rcm_count),
            0,
            int.MaxValue);

        var comments = commentsTask.ToList();

        var reviews = comments
            .Where(comment => comment.is_rate && comment.rate_star.HasValue)
            .OrderByDescending(comment => comment.created_at)
            .ThenByDescending(comment => comment.id)
            .Take(6)
            .Select(comment => new ComicDetailReviewResponse
            {
                id = comment._id,
                user_display_name = comment.User?.name ?? "Độc giả ẩn danh",
                rating = comment.rate_star!.Value,
                comment = comment.comment,
                created_at = comment.created_at
            })
            .ToList();

        var discussions = comments
            .Where(comment => !comment.is_rate && comment.reply_id == null)
            .OrderByDescending(comment => comment.created_at)
            .ThenByDescending(comment => comment.id)
            .Take(6)
            .Select(comment => new ComicDetailDiscussionResponse
            {
                id = comment._id,
                user_display_name = comment.User?.name ?? "Độc giả ẩn danh",
                message = comment.comment,
                created_at = comment.created_at
            })
            .ToList();

        var related = relatedTask
            .Where(other => other.id != comic.id && other.deleted_at == null && other.slug != comic.slug)
            .OrderByDescending(other => other.updated_at)
            .ThenBy(other => other.id)
            .Take(6)
            .Select(other => new ComicDetailRelatedComicResponse
            {
                id = other._id,
                slug = other.slug,
                title = other.name,
                cover_url = other.cover_url,
                latest_chapter = other.chapter_count
            })
            .ToList();

        var userLastReadChapter = await ResolveUserLastReadChapterAsync(userId, comic.id, chapterList);

        var categoryResponses = comic.ComicHaveCategories?
            .Where(relation => relation.ComicCategory != null)
            .Select(relation => new ComicDetailCategoryResponse
            {
                id = relation.ComicCategory!._id,
                name = relation.ComicCategory!.name
            })
            .ToList() ?? new List<ComicDetailCategoryResponse>();

        var detailComic = new ComicDetailComicResponse
        {
            id = comic._id,
            slug = comic.slug,
            title = comic.name,
            synopsis = comic.description,
            author_name = comic.author,
            cover_url = comic.cover_url,
            banner_url = comic.banner_url,
            rate = Math.Round(comic.rate, 2),
            rate_count = comic.rate_count,
            bookmark_count = comic.bookmark_count,
            weekly_chapter_count = weeklyChapterCount,
            weekly_recommendations = weeklyRecommendations,
            user_last_read_chapter = userLastReadChapter,
            categories = categoryResponses
        };

        var highlights = BuildComicHighlights(detailComic, comic);

        return new ComicDetailResponse
        {
            comic = detailComic,
            latest_chapters = latestChapters,
            advertisements = BuildComicAdvertisements(comic),
            introduction = comic.description,
            related_by_author = related,
            reviews = reviews,
            discussions = discussions,
            highlights = highlights
        };
    }

    public async Task<ComicChaptersListResponse?> GetComicChaptersBySlugAsync(string slug, long? userId = null)
    {
        var normalizedSlug = slug?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(normalizedSlug))
        {
            throw new UserRequestException("Slug truyện không hợp lệ");
        }

        var comic = await _comicRepository.GetBySlugAsync(normalizedSlug);
        if (comic == null || comic.deleted_at != null)
        {
            return null;
        }

        var chapterEntities = (await _comicChapterRepository.GetByComicIdAsync(comic.id))
            .Where(chapter => chapter.deleted_at == null)
            .OrderBy(chapter => chapter.chapter)
            .ThenBy(chapter => chapter.id)
            .ToList();

        var currentUtc = DateTime.UtcNow;
        var unlockedChapterIds = new HashSet<long>();
        if (userId.HasValue)
        {
            var unlockHistories = await _comicUnlockHistoryRepository.GetByUserIdAsync(userId.Value) ?? Enumerable.Empty<UserComicUnlockHistory>();
            unlockedChapterIds = unlockHistories
                .Where(history => history.comic_id == comic.id)
                .Select(history => history.comic_chapter_id)
                .ToHashSet();
        }

        var chapterResponses = chapterEntities
            .Select(chapter =>
            {
                var requireKey = chapter.key_require > 0 && (!chapter.key_require_until.HasValue || currentUtc <= chapter.key_require_until.Value);
                var unlocked = requireKey && unlockedChapterIds.Contains(chapter.id);
                return new ComicChapterListItemResponse
                {
                    id = chapter._id,
                    chapter = chapter.chapter,
                    title = BuildChapterTitle(chapter.chapter),
                    updated_at = chapter.updated_at,
                    is_locked = requireKey,
                    key_require = chapter.key_require,
                    is_unlocked = unlocked
                };
            })
            .ToList();

        var userLastReadChapter = await ResolveUserLastReadChapterAsync(userId, comic.id, chapterEntities);

        return new ComicChaptersListResponse
        {
            comic = new ComicChapterListComicResponse
            {
                id = comic._id,
                slug = comic.slug,
                title = comic.name,
                author_name = comic.author,
                cover_url = comic.cover_url
            },
            chapters = chapterResponses,
            total_chapters = chapterResponses.Count,
            user_last_read_chapter = userLastReadChapter
        };
    }

    public async Task<IEnumerable<ComicResponse>> SearchComicsAsync(string keyword, int limit, double minScore)
    {
        var normalizedKeyword = keyword?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKeyword))
            return [];

        limit = Math.Clamp(limit, 1, _embeddingService.Options.MaxResults);
        minScore = Math.Clamp(minScore, 0.0, 0.99);

        Vector? queryVector = null;
        var embeddingValues = await _embeddingService.CreateEmbeddingAsync(normalizedKeyword);
        if (embeddingValues is { Length: > 0 })
        {
            queryVector = new Vector(embeddingValues[0]);
        }

        var comics = await _comicRepository.SearchAsync(queryVector, normalizedKeyword, limit, minScore);
        return comics.Select(c => c.ToRespDTO());
    }

    public async Task<IEnumerable<ComicResponse>> GetComicsByAuthorAsync(string author)
    {
        var comics = await _comicRepository.GetByAuthorAsync(author);
        return comics.Select(c => c.ToRespDTO());
    }

    public async Task<IEnumerable<ComicResponse>> GetComicsByEmbeddedByAsync(long embeddedBy, int offset = 0, int limit = 50)
    {
        var comics = await _comicRepository.GetByEmbeddedByAsync(embeddedBy, offset, limit);
        return comics.Select(c => c.ToRespDTO());
    }

    public async Task<ComicResponse?> GetComicOwnedByAsync(long comicId, long ownerId)
    {
        var comic = await _comicRepository.GetByIdAndEmbeddedByAsync(comicId, ownerId);
        return comic?.ToRespDTO();
    }

    public Task<bool> IsComicOwnerAsync(long comicId, long ownerId)
    {
        return _comicRepository.ExistsAsync(comic => comic.id == comicId && comic.embedded_by == ownerId && comic.deleted_at == null);
    }

    public async Task<IEnumerable<ComicResponse>> GetComicsByEmbeddedBySlugAsync(string slug)
    {
        var comicEntity = await _comicRepository.GetBySlugAsync(slug);
        if (comicEntity == null)
            return Array.Empty<ComicResponse>();

        var embeddedBy = comicEntity.embedded_by;
        var comics = await _comicRepository.GetByEmbeddedByAsync(embeddedBy);
        var result = comics.Where(c => c.slug != slug).Select(c => c.ToRespDTO());
        return result;
    }

    public async Task<IEnumerable<ComicResponse>> GetComicsByStatusAsync(ComicStatus status)
    {
        var comics = await _comicRepository.GetByStatusAsync(status);
        return comics.Select(c => c.ToRespDTO());
    }

    public async Task<IEnumerable<ComicResponse>> GetComicsAsync(int offset, int limit)
    {
        var comics = await _comicRepository.GetPagedAsync(offset, limit);
        return comics.Select(c => c.ToRespDTO());
    }

    public async Task<ComicResponse> CreateComicAsync(CreateComicRequest comicRequest, long embedded_by)
    {
        comicRequest.name = comicRequest.name?.Trim();
        if (string.IsNullOrWhiteSpace(comicRequest.slug))
            comicRequest.slug = comicRequest.name?.ToSlug();
        else
            comicRequest.slug = comicRequest.slug.Trim().ToLower();
        // Kiểm tra slug đã tồn tại chưa
        if (await _comicRepository.ExistsAsync(c => c.slug == comicRequest.slug))
        {
            throw new UserRequestException("Slug truyện đã tồn tại. Vui lòng chọn slug khác.");
            // comicRequest.slug = $"{comicRequest.slug}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        }

        // Validate và set default main_category_id nếu không hợp lệ
        if (comicRequest.main_category_id <= 0 || comicRequest.main_category_id >= 2000)
        {
            comicRequest.main_category_id = 1001; // Default to "Tiên Hiệp"
        }

        // Chuyển đổi từ DTO sang Entity
        var comic = comicRequest.ToEntity();
        comic.embedded_by = embedded_by;
        // Ensure rate and chap_count are 0 for new comics (not set by frontend)
        comic.rate = 0;
        comic.chapter_count = 0;
        // var embeddingValues = await _embeddingService.CreateEmbeddingAsync($"{comic.name}, {comic.description}");
        // if (embeddingValues is { Length: > 0 })
        // {
        //     comic.search_vector = new Vector(embeddingValues[0]);
        // }

        // Thêm comic vào database
        var newComic = await _comicRepository.AddAsync(comic);

        // Thêm categories nếu có (chỉ chấp nhận categories có ID > 2000)
        // Mỗi category type chỉ được chọn 1 lần
        if (comicRequest.category_ids != null && comicRequest.category_ids.Any())
        {
            // Track categories by type to ensure only 1 per type
            var mainCharacterIds = new List<long>();
            var worldThemeIds = new List<long>();
            var classIds = new List<long>();
            var viewIds = new List<long>();

            // Validate và phân loại các categories
            foreach (var categoryId in comicRequest.category_ids)
            {
                // Chỉ chấp nhận categories có ID > 2000
                if (categoryId <= 2000)
                    continue; // Skip categories with ID <= 2000

                // Validate category exists
                var category = await _comicCategoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                    continue; // Skip invalid category IDs

                // Phân loại theo ID range
                if (categoryId >= 2001 && categoryId <= 2010)
                {
                    mainCharacterIds.Add(categoryId);
                }
                else if (categoryId >= 3001 && categoryId <= 3052)
                {
                    worldThemeIds.Add(categoryId);
                }
                else if (categoryId >= 4001 && categoryId <= 4038)
                {
                    classIds.Add(categoryId);
                }
                else if (categoryId >= 5001 && categoryId <= 5002)
                {
                    viewIds.Add(categoryId);
                }
            }

            // Chỉ thêm category đầu tiên của mỗi type (mỗi type chỉ được chọn 1)
            var categoriesToAdd = new List<long>();
            if (mainCharacterIds.Any())
                categoriesToAdd.Add(mainCharacterIds.First());
            if (worldThemeIds.Any())
                categoriesToAdd.Add(worldThemeIds.First());
            if (classIds.Any())
                categoriesToAdd.Add(classIds.First());
            if (viewIds.Any())
                categoriesToAdd.Add(viewIds.First());

            // Thêm các categories vào database
            foreach (var categoryId in categoriesToAdd)
            {
                // Kiểm tra đã tồn tại chưa (tránh duplicate)
                if (!await _comicHaveCategoryRepository.ExistsAsync(newComic.id, categoryId))
                {
                    await _comicHaveCategoryRepository.AddAsync(newComic.id, categoryId);
                }
            }
        }

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(newComic, newComic.id);

        return newComic.ToRespDTO();
    }

    public async Task<ComicResponse?> UpdateComicAsync(long id, UpdateComicRequest comicRequest)
    {
        // Lấy comic từ database
        var comic = await _comicRepository.GetByIdAsync(id);
        if (comic == null)
            return null;

        // Cập nhật thông tin
        if (comicRequest.name != comic.name || comicRequest.description != comic.description)
        {
            var embeddingValues = await _embeddingService.CreateEmbeddingAsync($"{comic.name}, {comic.description}");
            if (embeddingValues is { Length: > 0 })
            {
                comic.search_vector = new Vector(embeddingValues[0]);
            }
        }
        comic.UpdateFromRequest(comicRequest);
        await _comicHaveCategoryRepository.UpdateAllOfComicAsync(comic.id, comicRequest.category_ids ?? Enumerable.Empty<long>());
        // Cập nhật vào database
        await Task.WhenAll(
            _comicRepository.UpdateAsync(comic),
            _redisCache.AddOrUpdateInRedisAsync(comic, comic.id)
        );

        // Cập nhật cache

        return comic.ToRespDTO();
    }

    public async Task<bool> DeleteComicAsync(long id)
    {
        // Lấy comic từ database
        var comic = await _comicRepository.GetByIdAsync(id);
        if (comic == null)
            return false;

        // Soft delete: cập nhật deleted_at
        comic.deleted_at = DateTime.UtcNow;
        await _comicRepository.UpdateAsync(comic);

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);

        return true;
    }

    public async Task<UserHomeResponse> GetHomeForUserAsync(long userId)
    {
        const int historyLimit = 12;
        const int editorPickLimit = 6;
        const int rankingLimit = 10;
        const int updatesLimit = 12;
        const int completedLimit = 10;
        const int reviewLimit = 6;

        var historyTask = await BuildHistoryAsync(userId, historyLimit);
        var editorPicksTask = await BuildEditorPicksAsync(editorPickLimit);
        var recommendedTask = await BuildTopRecommendedAsync(rankingLimit);
        var weeklyReadsTask = await BuildTopWeeklyReadsAsync(rankingLimit);
        var updatesTask = await BuildLatestUpdatesAsync(updatesLimit);
        var completedTask = await BuildRecentlyCompletedAsync(completedLimit);
        var reviewsTask = await BuildLatestReviewsAsync(reviewLimit);

        return new UserHomeResponse
        {
            history = historyTask,
            editor_picks = editorPicksTask,
            top_recommended = recommendedTask,
            top_weekly_reads = weeklyReadsTask,
            latest_updates = updatesTask,
            recently_completed = completedTask,
            latest_reviews = reviewsTask
        };
    }

    private async Task<IReadOnlyList<UserHomeHistoryResponse>> BuildHistoryAsync(long userId, int limit)
    {
        var histories = (await _readHistoryRepository.GetByUserIdAsync(userId, limit)).ToList();
        if (histories.Count == 0)
        {
            return Array.Empty<UserHomeHistoryResponse>();
        }
        return histories
            .Select(history =>
            {
                return new UserHomeHistoryResponse
                {
                    comic_id = history.comic_id,
                    comic_title = history.Comic?.name ?? "Truyện không xác định",
                    comic_slug = history.Comic?.slug ?? string.Empty,
                    cover_url = history.Comic?.cover_url,
                    last_read_chapter = history.ComicChapter?.chapter ?? 0,
                    total_chapters = history.Comic?.chapter_count ?? 0,
                    last_read_at = history.updated_at
                };
            }).ToList();
    }

    private static ComicDetailAdvertisementsResponse BuildComicAdvertisements(Comic comic)
    {
        var fallbackImage = comic.banner_url
            ?? comic.cover_url
            ?? $"https://picsum.photos/seed/{comic.slug}/1200/360";

        ComicDetailAdvertisementResponse CreateAd(string variant, string label, string description)
        {
            return new ComicDetailAdvertisementResponse
            {
                id = $"{comic._id}-{variant}",
                image_url = fallbackImage,
                href = $"/comic/{comic.slug}",
                label = label,
                description = description
            };
        }

        return new ComicDetailAdvertisementsResponse
        {
            primary = CreateAd("primary", $"Khám phá {comic.name}", "Đọc chương mới nhất và theo dõi truyện mỗi ngày"),
            secondary = CreateAd("secondary", "Tham gia sự kiện đọc truyện", "Hoàn thành các chương để nhận thưởng và huy hiệu"),
            tertiary = CreateAd("tertiary", "Nâng cấp trải nghiệm đọc", "Mua gói premium để mở khóa toàn bộ nội dung")
        };
    }

    private static IReadOnlyList<string> BuildComicHighlights(ComicDetailComicResponse detailComic, Comic comic)
    {
        var highlights = new List<string>();

        if (comic.chapter_count > 0)
        {
            highlights.Add($"Đã phát hành {comic.chapter_count} chương");
        }

        if (detailComic.rate > 0 && detailComic.rate_count > 0)
        {
            highlights.Add($"Đánh giá {detailComic.rate:F1}/5 từ {detailComic.rate_count} lượt bình chọn");
        }

        if (detailComic.weekly_chapter_count > 0)
        {
            highlights.Add($"Cập nhật {detailComic.weekly_chapter_count} chương trong 7 ngày qua");
        }

        if (detailComic.weekly_recommendations > 0)
        {
            highlights.Add($"{detailComic.weekly_recommendations} lượt đề cử trong tháng này");
        }

        if (detailComic.categories.Count > 0)
        {
            var categories = string.Join(", ", detailComic.categories.Select(category => category.name));
            highlights.Add($"Thể loại tiêu biểu: {categories}");
        }
        else if (!string.IsNullOrWhiteSpace(comic.MainCategory?.name))
        {
            highlights.Add($"Thể loại chính: {comic.MainCategory!.name}");
        }

        return highlights.Count > 0 ? highlights : new List<string> { "Nội dung đang được cập nhật." };
    }

    private async Task<IReadOnlyList<UserHomeHighlightedComicResponse>> BuildEditorPicksAsync(int limit)
    {
        var comics = (await _comicRepository.GetTopRatedAsync(limit * 2))
            .Where(comic => comic.deleted_at == null)
            .Take(limit)
            .ToList();

        if (comics.Count == 0)
        {
            return Array.Empty<UserHomeHighlightedComicResponse>();
        }

        return comics.Select(comic => new UserHomeHighlightedComicResponse
        {
            comic_id = comic.id,
            comic_title = comic.name,
            comic_slug = comic.slug,
            cover_url = comic.cover_url,
            short_description = BuildShortDescription(comic.description),
            latest_chapter = comic.chapter_count,
            average_rating = Math.Round(comic.rate, 2)
        }).ToList();
    }

    private async Task<IReadOnlyList<UserHomeRankingComicResponse>> BuildTopRecommendedAsync(int limit)
    {
        var lookupDate = DateTime.UtcNow.AddDays(-30);
        var aggregates = (await _readHistoryRepository.GetTopByUpdatedAtAsync(lookupDate, limit * 2)).ToList();
        if (aggregates.Count == 0)
        {
            return Array.Empty<UserHomeRankingComicResponse>();
        }
        return aggregates
            .Select(item =>
            {
                return new UserHomeRankingComicResponse
                {
                    comic_id = item.comic_id,
                    comic_title = item.Comic?.name ?? "Truyện không xác định",
                    comic_slug = item.Comic?.slug ?? string.Empty,
                    cover_url = item.Comic?.cover_url,
                    total_views = item.reader_count,
                    weekly_views = item.reader_count,
                    recommendation_score = 0
                };
            })
            .Take(limit)
            .ToList();
    }

    private async Task<IReadOnlyList<UserHomeRankingComicResponse>> BuildTopWeeklyReadsAsync(int limit)
    {
        var fromUtc = DateTime.UtcNow.AddDays(-7);
        var aggregates = (await _readHistoryRepository.GetTopByUpdatedAtAsync(fromUtc, limit * 2)).ToList();
        if (aggregates.Count == 0)
        {
            return Array.Empty<UserHomeRankingComicResponse>();
        }
        return aggregates
            .Select(item => new UserHomeRankingComicResponse
            {
                comic_id = item.comic_id,
                comic_title = item.Comic.slug,
                cover_url = item.Comic.cover_url,
                total_views = item.reader_count,
                weekly_views = item.reader_count,
                recommendation_score = 0
            })
            .Take(limit)
            .ToList();
    }

    private async Task<int?> ResolveUserLastReadChapterAsync(long? userId, long comicId, IReadOnlyCollection<ComicChapter> cachedChapters)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        var history = await _readHistoryRepository.GetByUserAndComicAsync(userId.Value, comicId);
        if (history == null)
        {
            return null;
        }

        if (history.ComicChapter?.chapter != null)
        {
            return history.ComicChapter.chapter;
        }

        var chapterFromCache = cachedChapters.FirstOrDefault(chapter => chapter.id == history.chapter_id);
        if (chapterFromCache != null)
        {
            return chapterFromCache.chapter;
        }

        var chapterEntity = await _comicChapterRepository.GetByIdAsync(history.chapter_id);
        return chapterEntity?.chapter;
    }

    private async Task<IReadOnlyList<UserHomeComicUpdateResponse>> BuildLatestUpdatesAsync(int limit)
    {
        var chapters = (await _comicChapterRepository.GetLatestUpdatedAsync(limit * 2)).ToList();
        if (chapters.Count == 0)
        {
            return Array.Empty<UserHomeComicUpdateResponse>();
        }

        return chapters
            .Where(chapter => chapter.Comic != null)
            .OrderByDescending(chapter => chapter.updated_at)
            .ThenByDescending(chapter => chapter.id)
            .Take(limit)
            .Select(chapter => new UserHomeComicUpdateResponse
            {
                comic_id = chapter.comic_id,
                comic_title = chapter.Comic!.name,
                comic_slug = chapter.Comic.slug,
                chapter_title = BuildChapterTitle(chapter.chapter),
                chapter_number = chapter.chapter,
                updated_at = chapter.updated_at
            })
            .ToList();
    }

    private async Task<IReadOnlyList<UserHomeCompletedComicResponse>> BuildRecentlyCompletedAsync(int limit)
    {
        var comics = (await _comicRepository.FindAsync(comic => comic.status == ComicStatus.Completed && comic.deleted_at == null))
            .OrderByDescending(comic => comic.updated_at)
            .ThenByDescending(comic => comic.id)
            .Take(limit)
            .ToList();

        if (comics.Count == 0)
        {
            return Array.Empty<UserHomeCompletedComicResponse>();
        }

        return comics.Select(comic => new UserHomeCompletedComicResponse
        {
            comic_id = comic.id,
            comic_title = comic.name,
            comic_slug = comic.slug,
            cover_url = comic.cover_url,
            total_chapters = comic.chapter_count,
            completed_at = comic.updated_at
        }).ToList();
    }

    private async Task<IReadOnlyList<UserHomeReviewResponse>> BuildLatestReviewsAsync(int limit)
    {
        var comments = (await _comicCommentRepository.GetLatestRatingReviewsAsync(limit * 2)).ToList();
        if (comments.Count == 0)
        {
            return Array.Empty<UserHomeReviewResponse>();
        }

        return comments
            .Where(comment => comment.Comic != null)
            .OrderByDescending(comment => comment.created_at)
            .ThenByDescending(comment => comment.id)
            .Take(limit)
            .Select(comment => new UserHomeReviewResponse
            {
                review_id = comment.id,
                comic_id = comment.comic_id,
                comic_title = comment.Comic!.name,
                comic_slug = comment.Comic.slug,
                user_display_name = comment.User?.name ?? "Độc giả ẩn danh",
                rating = comment.rate_star ?? 0,
                liked_count = comment.like,
                created_at = comment.created_at,
                content = comment.comment
            })
            .ToList();
    }
    private static string? BuildShortDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        const int maxLength = 160;
        var trimmed = description.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength] + "...";
    }

    private static string BuildChapterTitle(int chapterNumber)
    {
        return $"Chương {chapterNumber}";
    }
}
