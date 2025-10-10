using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using TruyenCV.DTO.Response;
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

    public UserComicBookmarkService(
        IUserComicBookmarkRepository bookmarkRepository,
        IComicRepository comicRepository,
        IDistributedCache redisCache)
    {
        _bookmarkRepository = bookmarkRepository;
        _comicRepository = comicRepository;
        _redisCache = redisCache;
    }

    public async Task<IEnumerable<UserComicBookmarkResponse>> GetBookmarksByUserIdAsync(ulong userId)
    {
        var bookmarks = await _bookmarkRepository.GetByUserIdAsync(userId);
        return bookmarks.Select(b => b.ToRespDTO());
    }

    public async Task<UserComicBookmarkResponse> CreateBookmarkAsync(ulong userId, ulong comicId)
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

    public async Task<bool> IsBookmarkedAsync(ulong userId, ulong comicId)
    {
        var existing = await _bookmarkRepository.GetByUserAndComicAsync(userId, comicId);
        return existing != null;
    }

    public async Task<bool> RemoveBookmarkAsync(ulong userId, ulong comicId)
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

    private async Task UpdateComicBookmarkCount(ulong comicId, int delta)
    {
        var comic = await _comicRepository.GetByIdAsync(comicId);
        if (comic == null)
        {
            return;
        }

        var newValue = (int)comic.bookmark_count + delta;
        comic.bookmark_count = (uint)Math.Max(0, newValue);
        comic.updated_at = DateTime.UtcNow;

    await _comicRepository.UpdateAsync(comic);
    }

    private async Task InvalidateCaches(ulong userId, ulong comicId)
    {
        await _redisCache.RemoveAsync($"UserComicBookmark:user:{userId}");
        await _redisCache.RemoveAsync($"UserComicBookmark:comic:{comicId}");
        await _redisCache.RemoveAsync($"UserComicBookmark:one:user:{userId}:comic:{comicId}");
    }
}
