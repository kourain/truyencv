using System;
using System.Linq;
using TruyenCV;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public class ComicReadingService : IComicReadingService
{
    private readonly IComicService _comicService;
    private readonly IComicChapterService _comicChapterService;
    private readonly IComicRecommendService _comicRecommendService;
    private readonly IUserComicUnlockHistoryService _unlockHistoryService;
    private readonly IUserComicReadHistoryService _readHistoryService;

    public ComicReadingService(
        IComicService comicService,
        IComicChapterService comicChapterService,
        IComicRecommendService comicRecommendService,
        IUserComicUnlockHistoryService unlockHistoryService,
        IUserComicReadHistoryService readHistoryService)
    {
        _comicService = comicService;
        _comicChapterService = comicChapterService;
        _comicRecommendService = comicRecommendService;
        _unlockHistoryService = unlockHistoryService;
        _readHistoryService = readHistoryService;
    }

    public async Task<ComicChapterReadResponse?> GetChapterAsync(string slug, int chapterNumber, long userId)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new UserRequestException("Slug truyện không hợp lệ");
        }

        if (chapterNumber <= 0)
        {
            throw new UserRequestException("Số chương không hợp lệ");
        }

        var comic = await _comicService.GetComicBySlugAsync(slug);
        if (comic == null)
        {
            throw new UserRequestException("Không tìm thấy truyện");
        }

        var comicId = comic.id.ToSnowflakeId(nameof(comic.id));
        var chapter = await _comicChapterService.GetChapterByComicIdAndChapterAsync(comicId, chapterNumber);
        if (chapter == null)
        {
            return null;
        }

        var chapterId = chapter.id.ToSnowflakeId(nameof(chapter.id));
        var requireKey = chapter.key_require > 0 && (!chapter.key_require_until.HasValue || DateTime.UtcNow <= chapter.key_require_until.Value);
        if (requireKey)
        {
            var unlocked = await _unlockHistoryService.HasUnlockedChapterAsync(userId, chapterId);
            if (!unlocked)
            {
                throw new UserRequestException("Bạn cần mở khóa chương này trước khi đọc");
            }
        }

        var previous = await _comicChapterService.GetPreviousChapterAsync(comicId, chapterNumber);
        var next = await _comicChapterService.GetNextChapterAsync(comicId, chapterNumber);

        var now = DateTime.UtcNow;
        var currentMonthRecommend = await _comicRecommendService.GetByComicAndPeriodAsync(comicId, now.Month, now.Year);
        var hasRecommended = await _comicRecommendService.HasUserRecommendedAsync(comicId, userId);
        var topRecommend = await _comicRecommendService.GetTopAsync(now.Month, now.Year, 5);

        string? recommendedTitle = null;
        string? recommendedSlug = null;
        var alternative = topRecommend.FirstOrDefault(rcm => rcm.comic_id != comic.id);
        if (alternative != null)
        {
            var altComicId = alternative.comic_id.ToSnowflakeId();
            var altComic = await _comicService.GetComicByIdAsync(altComicId);
            if (altComic != null)
            {
                recommendedTitle = altComic.name;
                recommendedSlug = altComic.slug;
            }
        }

        return new ComicChapterReadResponse
        {
            comic_id = comic.id,
            comic_slug = comic.slug,
            comic_title = comic.name,
            author_name = comic.author,
            chapter_id = chapter.id,
            chapter_number = chapter.chapter,
            chapter_title = $"Chương {chapter.chapter}",
            content = chapter.content,
            updated_at = chapter.updated_at,
            previous_chapter_number = previous?.chapter,
            previous_chapter_id = previous?.id,
            next_chapter_number = next?.chapter,
            next_chapter_id = next?.id,
            recommended_comic_title = recommendedTitle,
            recommended_comic_slug = recommendedSlug,
            monthly_recommendations = currentMonthRecommend?.rcm_count ?? 0,
            month = currentMonthRecommend?.month ?? now.Month,
            year = currentMonthRecommend?.year ?? now.Year,
            has_recommended = hasRecommended
        };
    }

    public async Task RecordChapterReadAsync(long comicId, int chapterNumber, long userId)
    {
        if (chapterNumber <= 0)
        {
            throw new UserRequestException("Số chương không hợp lệ");
        }
        var chapter = await _comicChapterService.GetChapterByComicIdAndChapterAsync(comicId, chapterNumber);
        if (chapter == null)
        {
            return;
        }

        var chapterId = chapter.id.ToSnowflakeId(nameof(chapter.id));
        await _readHistoryService.UpsertReadHistoryAsync(userId, comicId, chapterId);
    }
}
