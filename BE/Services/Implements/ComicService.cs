using System;
using System.Collections.Generic;
using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
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

    public ComicService(IComicRepository comicRepository, IDistributedCache redisCache, ITextEmbeddingService embeddingService)
    {
        _comicRepository = comicRepository;
        _redisCache = redisCache;
        _embeddingService = embeddingService;
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
}
