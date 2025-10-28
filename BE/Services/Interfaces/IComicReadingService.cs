using TruyenCV.DTOs.Response;

namespace TruyenCV.Services;

public interface IComicReadingService
{
    Task<ComicChapterReadResponse?> GetChapterAsync(string slug, int chapterNumber, long userId);
}
