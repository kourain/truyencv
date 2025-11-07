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

    public ComicService(
        IComicRepository comicRepository,
        IDistributedCache redisCache,
        ITextEmbeddingService embeddingService,
        IUserComicReadHistoryRepository readHistoryRepository,
        IComicChapterRepository comicChapterRepository,
        IComicRecommendRepository comicRecommendRepository,
        IComicCommentRepository comicCommentRepository)
    {
        _comicRepository = comicRepository;
        _redisCache = redisCache;
        _embeddingService = embeddingService;
        _readHistoryRepository = readHistoryRepository;
        _comicChapterRepository = comicChapterRepository;
        _comicRecommendRepository = comicRecommendRepository;
        _comicCommentRepository = comicCommentRepository;
    }

    public async Task<ComicResponse?> GetComicByIdAsync(long id)
    {
        var comic = await _comicRepository.GetByIdAsync(id);
        return comic?.ToRespDTO();
    }

    public async Task<ComicResponse?> GetComicBySlugAsync(string slug)
    {
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

    public async Task<ComicResponse?> GetComicDetailBySlugAsync(string slug)
    {
        var comic = await _comicRepository.GetBySlugAsync(slug);
        if (comic == null || comic.deleted_at != null)
        {
            return null;
        }

        return comic.ToRespDTO();
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

    public async Task<IEnumerable<ComicResponse>> GetComicsByEmbeddedByAsync(long embeddedBy)
    {
        var comics = await _comicRepository.GetByEmbeddedByAsync(embeddedBy);
        return comics.Select(c => c.ToRespDTO());
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
            comicRequest.slug = $"{comicRequest.slug}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        // Chuyển đổi từ DTO sang Entity
        var comic = comicRequest.ToEntity();
        comic.embedded_by = embedded_by;
        var embeddingValues = await _embeddingService.CreateEmbeddingAsync($"{comic.name}, {comic.description}");
        if (embeddingValues is { Length: > 0 })
        {
            comic.search_vector = new Vector(embeddingValues[0]);
        }

        // Thêm comic vào database
        var newComic = await _comicRepository.AddAsync(comic);

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

        // Cập nhật vào database
        await _comicRepository.UpdateAsync(comic);

        // Cập nhật cache
        await _redisCache.AddOrUpdateInRedisAsync(comic, comic.id);

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
                    cover_url = history.Comic?.cover_url,
                    last_read_chapter = history.ComicChapter?.chapter ?? 0,
                    total_chapters = history.Comic?.chapter_count ?? 0,
                    last_read_at = history.updated_at
                };
            }).ToList();
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
            .Select(item => new UserHomeRankingComicResponse
            {
                comic_id = item.comic_id,
                comic_title = string.Empty,
                cover_url = string.Empty,
                total_views = item.reader_count,
                weekly_views = item.reader_count,
                recommendation_score = 0
            })
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
                comic_title = string.Empty,
                cover_url = string.Empty,
                total_views = item.reader_count,
                weekly_views = item.reader_count,
                recommendation_score = 0
            })
            .ToList();
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
