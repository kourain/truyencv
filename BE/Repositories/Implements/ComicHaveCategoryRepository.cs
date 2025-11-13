using TruyenCV.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Implementation repository cho ComicHaveCategory entity
/// </summary>
public class ComicHaveCategoryRepository : IComicHaveCategoryRepository
{
    private readonly AppDataContext _context;
    private readonly DbSet<ComicHaveCategory> _dbSet;
    private readonly IDistributedCache _redisCache;
    private const double DefaultCacheMinutes = 30;

    public ComicHaveCategoryRepository(AppDataContext context, IDistributedCache redisCache)
    {
        _context = context;
        _dbSet = context.Set<ComicHaveCategory>();
        _redisCache = redisCache;
    }

    public async Task<IEnumerable<ComicCategory>> GetCategoriesByComicIdAsync(long comicId)
    {
        return await _redisCache.GetFromRedisAsync<ComicCategory>(
            () => _dbSet.AsNoTracking()
                .Where(chc => chc.comic_id == comicId)
                .Select(chc => chc.ComicCategory!)
                .ToListAsync(),
            $"comic:{comicId}:categories",
            DefaultCacheMinutes
        );
    }

    public async Task<IEnumerable<Comic>> GetComicsByCategoryIdAsync(long categoryId)
    {
        return await _redisCache.GetFromRedisAsync<Comic>(
            () => _dbSet.AsNoTracking()
                .Where(chc => chc.comic_category_id == categoryId)
                .Select(chc => chc.Comic!)
                .ToListAsync(),
            $"category:{categoryId}:comics",
            DefaultCacheMinutes
        );
    }

    public async Task<ComicHaveCategory> AddAsync(long comicId, long categoryId)
    {
        var entity = new ComicHaveCategory
        {
            comic_id = comicId,
            comic_category_id = categoryId
        };

        await _dbSet.AddAsync(entity);
        await Task.WhenAll(
            _context.SaveChangesAsync(),
            _redisCache.RemoveAsync($"comic:{comicId}:categories"),
            _redisCache.RemoveAsync($"category:{categoryId}:comics")
        );

        return entity;
    }

    public async Task<bool> DeleteAsync(long comicId, long categoryId)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(chc =>
            chc.comic_id == comicId && chc.comic_category_id == categoryId);

        if (entity == null)
            return false;
        _context.NotSoftDelete();
        _dbSet.Remove(entity);
        await Task.WhenAll(
            _context.SaveChangesAsync(),
            _redisCache.RemoveAsync($"comic:{comicId}:categories"),
            _redisCache.RemoveAsync($"category:{categoryId}:comics")
        );
        return true;
    }
    public async Task<int> UpdateAllOfComicAsync(long comicId, IEnumerable<long> newCategoryIds)
    {
        var existingEntities = await _dbSet.Where(chc => chc.comic_id == comicId).ToListAsync();
        var existingCategoryIds = existingEntities.Select(e => e.comic_category_id).ToHashSet();
        var newCategoryIdSet = newCategoryIds.ToHashSet();

        var toAdd = newCategoryIdSet.Except(existingCategoryIds)
            .Select(catId => new ComicHaveCategory
            {
                comic_id = comicId,
                comic_category_id = catId
            }).ToList();

        var toRemove = existingEntities
            .Where(e => !newCategoryIdSet.Contains(e.comic_category_id))
            .ToList();

        if (toAdd.Count == 0 && toRemove.Count == 0)
            return 0;

        _context.NotSoftDelete();
        if (toAdd.Count > 0)
            await _dbSet.AddRangeAsync(toAdd);
        if (toRemove.Count > 0)
            _dbSet.RemoveRange(toRemove);

        await Task.WhenAll(
            _context.SaveChangesAsync(),
            _redisCache.RemoveAsync($"comic:{comicId}:categories"),
            _redisCache.RemoveAsync($"comic:{comicId}")
        );

        return toAdd.Count + toRemove.Count;
    }

    public async Task<int> DeleteAllOfComicAsync(long comicId)
    {
        var entities = await _dbSet.Where(chc => chc.comic_id == comicId).ToListAsync();
        if (entities.Count == 0)
            return 0;
        _context.NotSoftDelete();
        _dbSet.RemoveRange(entities);
        await Task.WhenAll(
            _context.SaveChangesAsync(),
            _redisCache.RemoveAsync($"comic:{comicId}:categories"),
            _redisCache.RemoveAsync($"comic:{comicId}")
        );
        return entities.Count;
    }
    public async Task<bool> ExistsAsync(long comicId, long categoryId)
    {
        return await _dbSet.AnyAsync(chc =>
            chc.comic_id == comicId && chc.comic_category_id == categoryId);
    }
}
