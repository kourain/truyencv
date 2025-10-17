using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IComicRecommendService
{
    Task<ComicRecommendResponse> CreateOrUpdateAsync(CreateComicRecommendRequest request);
    Task<ComicRecommendResponse?> UpdateAsync(long id, UpdateComicRecommendRequest request);
    Task<IEnumerable<ComicRecommendResponse>> GetTopAsync(int month, int year, int limit);
    Task<IEnumerable<ComicRecommendResponse>> GetByComicAsync(long comicId, int limit);
    Task<ComicRecommendResponse?> GetByComicAndPeriodAsync(long comicId, int month, int year);
    Task<ComicRecommendResponse> RecommendAsync(long comicId, long userId);
}
