using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    // Convert CreateComicRequest to Comic entity
    public static Comic ToEntity(this CreateComicRequest request)
    {
        return new Comic
        {
            name = request.name,
            description = request.description,
            slug = request.slug ?? string.Empty, // Will be set by service if null
            author = request.author,
            author_slug = request.author.ToSlug(),
            embedded_from = request.embedded_from,
            embedded_from_url = request.embedded_from_url,
            cover_url = request.cover_url,
            banner_url = request.banner_url,
            chapter_count = 0, // Always 0 for new comics - auto-calculated
            main_category_id = request.main_category_id,
            bookmark_count = 0,
            rate = 0, // Always 0 for new comics - auto-calculated from user ratings
            status = request.status,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }
    private static Dictionary<long, string> categoryDict = new Dictionary<long, string>()
    {
        {1001, "Tiên Hiệp"},
        {1002, "Huyền Huyễn"},
        {1003, "Khoa Huyễn"},
        {1004, "Võng Du"},
        {1005, "Đô Thị"},
        {1006, "Đồng Nhân"},
        {1007, "Dã Sử"},
        {1008, "Cạnh Kỹ"},
        {1009, "Huyền Nghi"},
        {1010, "Kiếm Hiệp"},
        {1011, "Kỳ Ảo"},
        {1012, "Light Novel"}
    };
    // Convert Comic entity to ComicResponse
    public static ComicResponse ToRespDTO(this Comic comic)
    {
        return new ComicResponse
        {
            id = comic._id,
            name = comic.name,
            description = comic.description,
            slug = comic.slug,
            author = comic.author,
            embedded_from = comic.embedded_from,
            embedded_from_url = comic.embedded_from_url,
            cover_url = comic.cover_url,
            banner_url = comic.banner_url,
            chap_count = (int)comic.chapter_count,
            bookmark_count = comic.bookmark_count,
            rate_count = comic.rate_count,
            main_category = categoryDict.TryGetValue(comic.main_category_id, out var categoryName) ? categoryName : "Unknown",
            rate = comic.rate,
            status = comic.status,
            created_at = comic.created_at,
            updated_at = comic.updated_at
        };
    }

    // Update Comic entity from UpdateComicRequest
    public static void UpdateFromRequest(this Comic comic, UpdateComicRequest request)
    {
        comic.name = request.name;
        comic.description = request.description;
        comic.slug = request.slug;
        comic.author = request.author;
        comic.author_slug = request.author.ToSlug();
        comic.embedded_from = request.embedded_from;
        comic.embedded_from_url = request.embedded_from_url;
        comic.cover_url = request.cover_url;
        comic.banner_url = request.banner_url;
        comic.chapter_count = (int)request.chap_count;
        comic.main_category_id = request.main_category_id;
        comic.rate = request.rate;
        comic.status = request.status;
        comic.updated_at = DateTime.UtcNow;
    }
}
