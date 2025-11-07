using System;
using System.Linq;
using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicComment entity
/// </summary>
public class ComicCommentRepository : Repository<ComicComment>, IComicCommentRepository
{
    public ComicCommentRepository(AppDataContext context, IDistributedCache redisCache) : base(context, redisCache)
    {
    }

    public async Task<ComicComment?> GetByIdAsync(long id)
    {
        return await _redisCache.GetFromRedisAsync<ComicComment>(
            () => _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.id == id),
            id,
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<ComicComment>> GetByComicIdAsync(long comicId)
    {
        return await _redisCache.GetFromRedisAsync<ComicComment>(
            () => _dbSet.AsNoTracking()
                .Where(c => c.comic_id == comicId)
                .OrderByDescending(c => c.created_at)
                .ToListAsync(),
            $"comic:{comicId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<ComicComment>> GetByChapterIdAsync(long chapterId)
    {
        return await _redisCache.GetFromRedisAsync<ComicComment>(
            () => _dbSet.AsNoTracking()
                .Where(c => c.comic_chapter_id == chapterId)
                .OrderByDescending(c => c.created_at)
                .ToListAsync(),
            $"chapter:{chapterId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<ComicComment>> GetByUserIdAsync(long userId)
    {
        return await _redisCache.GetFromRedisAsync<ComicComment>(
            () => _dbSet.AsNoTracking()
                .Where(c => c.user_id == userId)
                .OrderByDescending(c => c.created_at)
                .ToListAsync(),
            $"user:{userId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<ComicComment>> GetRepliesAsync(long commentId)
    {
        return await _redisCache.GetFromRedisAsync<ComicComment>(
            () => _dbSet.AsNoTracking()
                .Where(c => c.reply_id == commentId)
                .OrderBy(c => c.created_at)
                .ToListAsync(),
            $"replies:{commentId}",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<ComicComment>> GetLatestRatingReviewsAsync(int limit)
    {
        limit = Math.Clamp(limit, 1, 50);

        return await _redisCache.GetFromRedisAsync(() => _dbSet.AsNoTracking()
            .Include(comment => comment.User)
            .Include(comment => comment.Comic)
            .Where(comment => comment.deleted_at == null && comment.is_rate && !comment.is_hidden)
            .OrderByDescending(comment => comment.created_at)
            .ThenByDescending(comment => comment.id)
            .Take(limit)
            .ToListAsync(), DefaultCacheMinutes);
    }
}