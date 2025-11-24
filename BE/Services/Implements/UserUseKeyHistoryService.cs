using System.Collections.Generic;
using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;
using TruyenCV.Repositories;

namespace TruyenCV.Services;

public class UserUseKeyHistoryService : IUserUseKeyHistoryService
{
    private readonly IUserUseKeyHistoryRepository _userUseKeyHistoryRepository;
    private readonly IComicChapterRepository _comicChapterRepository;
    private readonly IComicRepository _comicRepository;

    public UserUseKeyHistoryService(
        IUserUseKeyHistoryRepository userUseKeyHistoryRepository,
        IComicChapterRepository comicChapterRepository,
        IComicRepository comicRepository)
    {
        _userUseKeyHistoryRepository = userUseKeyHistoryRepository;
        _comicChapterRepository = comicChapterRepository;
        _comicRepository = comicRepository;
    }

    public async Task<IEnumerable<UserUseKeyHistoryResponse>> GetByUserIdAsync(long userId)
    {
        var histories = (await _userUseKeyHistoryRepository.GetByUserIdAsync(userId))
            .Where(history => history.deleted_at == null)
            .ToList();

        if (histories.Count == 0)
        {
            return Enumerable.Empty<UserUseKeyHistoryResponse>();
        }

        var responses = histories.Select(history => history.ToRespDTO()).ToList();
        var chapterIds = histories
            .Select(history => history.chapter_id)
            .Where(chapterId => chapterId.HasValue)
            .Select(chapterId => chapterId!.Value)
            .Distinct()
            .ToList();

        if (chapterIds.Count == 0)
        {
            return responses;
        }

        var chapterResults = await chapterIds.SelectAsync(id => _comicChapterRepository.GetByIdAsync(id));

        if (chapterResults.Count() == 0)
        {
            return responses;
        }

        var chapterLookup = chapterResults.ToDictionary(chapter => chapter.id, chapter => chapter);
        var comicIds = chapterResults.Select(chapter => chapter.comic_id).Distinct().ToList();
        var comicLookup = new Dictionary<long, string>();

        if (comicIds.Count > 0)
        {
            var comicResults = await comicIds.SelectAsync(id => _comicRepository.GetByIdAsync(id));
            comicLookup = comicResults
                .Where(comic => comic != null)
                .Cast<Comic>()
                .ToDictionary(comic => comic.id, comic => comic.name);
        }

        foreach (var response in responses)
        {
            if (!long.TryParse(response.chapter_id, out var chapterId))
            {
                continue;
            }

            if (!chapterLookup.TryGetValue(chapterId, out var chapter))
            {
                continue;
            }

            response.chapter_number = chapter.chapter;

            if (comicLookup.TryGetValue(chapter.comic_id, out var comicName))
            {
                response.comic_name = comicName;
            }
        }

        return responses;
    }

    public async Task<UserUseKeyHistoryResponse> CreateAsync(CreateUserUseKeyHistoryRequest request)
    {
        var entity = request.ToEntity();
        var created = await _userUseKeyHistoryRepository.AddAsync(entity);
        return created.ToRespDTO();
    }
}
