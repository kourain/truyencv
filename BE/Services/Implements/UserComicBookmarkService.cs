using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

/// <summary>
/// Implementation của UserComicBookmark Service
/// </summary>
public class UserComicBookmarkService : IUserComicBookmarkService
{
    private readonly IUserComicBookmarkRepository _bookmarkRepository;
    private readonly IComicRepository _comicRepository;
    private readonly IDistributedCache _redisCache;
    private readonly AppDataContext _dbContext;

    public UserComicBookmarkService(
        IUserComicBookmarkRepository bookmarkRepository,
        IComicRepository comicRepository,
        IDistributedCache redisCache,
        AppDataContext dbContext)
    {
        _bookmarkRepository = bookmarkRepository;
        _comicRepository = comicRepository;
        _redisCache = redisCache;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserComicBookmarkResponse>> GetBookmarksByUserIdAsync(long userId)
    {
        var bookmarks = await _bookmarkRepository.GetByUserIdAsync(userId);
        return bookmarks.Select(b => b.ToRespDTO());
    }

    public async Task<IEnumerable<UserBookmarkWithComicDetailResponse>> GetBookmarksWithComicDetailsAsync(long userId)
    {
        var bookmarks = await _bookmarkRepository.GetByUserIdAsync(userId);
        var results = new List<UserBookmarkWithComicDetailResponse>();

        foreach (var bookmark in bookmarks)
        {
            var comic = await _comicRepository.GetByIdAsync(bookmark.comic_id);
            if (comic == null || comic.deleted_at != null)
            {
                continue;
            }

            // Get latest chapter number
            var latestChapter = await _dbContext.ComicChapters
                .Where(c => c.comic_id == comic.id && c.deleted_at == null)
                .OrderByDescending(c => c.chapter)
                .Select(c => c.chapter)
                .FirstOrDefaultAsync();

            // Get user's last read chapter for this comic
            var lastReadChapter = await _dbContext.UserComicReadHistories
                .Where(h => h.user_id == userId && h.comic_id == comic.id)
                .Include(h => h.ComicChapter)
                .OrderByDescending(x => x.updated_at)
                .Select(x => x.ComicChapter!.chapter)
                .FirstOrDefaultAsync();

            results.Add(new UserBookmarkWithComicDetailResponse
            {
                id = bookmark.id.ToString(),
                comic_id = comic.id.ToString(),
                comic_title = comic.name,
                comic_slug = comic.slug,
                comic_cover_url = comic.cover_url,
                user_last_read_chapter = lastReadChapter > 0 ? lastReadChapter : null,
                latest_chapter_number = latestChapter,
                bookmarked_at = bookmark.created_at
            });
        }

        return results.OrderByDescending(r => r.bookmarked_at);
    }

    public async Task<UserComicBookmarkResponse> CreateBookmarkAsync(long userId, long comicId)
    {
        var existing = await _bookmarkRepository.GetByUserAndComicAsync(userId, comicId);
        if (existing != null)
        {
            return existing.ToRespDTO();
        }

        var bookmark = new UserComicBookmark
        {
            user_id = userId,
            comic_id = comicId,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        var newBookmark = await _bookmarkRepository.AddAsync(bookmark);

        await UpdateComicBookmarkCount(comicId, 1);
        await InvalidateCaches(userId, comicId);

        return newBookmark.ToRespDTO();
    }

    public async Task<bool> IsBookmarkedAsync(long userId, long comicId)
    {
        var existing = await _bookmarkRepository.GetByUserAndComicAsync(userId, comicId);
        return existing != null;
    }

    public async Task<bool> RemoveBookmarkAsync(long userId, long comicId)
    {
        var bookmark = await _bookmarkRepository.GetByUserAndComicAsync(userId, comicId);
        if (bookmark == null)
        {
            return false;
        }

        await _bookmarkRepository.DeleteAsync(bookmark);
        await UpdateComicBookmarkCount(comicId, -1);
        await InvalidateCaches(userId, comicId);

        return true;
    }

    private async Task UpdateComicBookmarkCount(long comicId, int delta)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            return;
        }

        var newValue = (int)comic.bookmark_count + delta;
        comic.bookmark_count = (int)Math.Max(0, newValue);

        await _comicRepository.UpdateAsync(comic);
    }

    private async Task InvalidateCaches(long userId, long comicId)
    {
        await _redisCache.RemoveAsync($"UserComicBookmark:user:{userId}");
        await _redisCache.RemoveAsync($"UserComicBookmark:comic:{comicId}");
        await _redisCache.RemoveAsync($"UserComicBookmark:one:user:{userId}:comic:{comicId}");
    }
}
