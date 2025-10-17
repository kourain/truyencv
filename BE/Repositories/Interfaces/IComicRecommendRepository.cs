using TruyenCV.Models;

namespace TruyenCV.Repositories;

public interface IComicRecommendRepository : IRepository<ComicRecommend>
{
    Task<ComicRecommend?> GetByIdAsync(long id);
    Task<ComicRecommend?> GetByComicAndPeriodAsync(long comicId, int month, int year);
    Task<ComicRecommend?> GetTrackedByComicAndPeriodAsync(long comicId, int month, int year);
    Task<IEnumerable<ComicRecommend>> GetTopAsync(int month, int year, int limit);
    Task<IEnumerable<ComicRecommend>> GetByComicAsync(long comicId, int limit);
}
