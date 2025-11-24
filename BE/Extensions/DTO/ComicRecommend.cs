using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static ComicRecommend ToEntity(this CreateComicRecommendRequest request)
    {
        return new ComicRecommend
        {
            comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
            rcm_count = request.rcm_count,
            month = request.month,
            year = request.year
        };
    }

    public static ComicRecommendResponse ToRespDTO(this ComicRecommend recommend)
    {
        return new ComicRecommendResponse
        {
            id = recommend._id,
            comic_id = recommend.comic_id.ToString(),
            rcm_count = recommend.rcm_count,
            month = recommend.month,
            year = recommend.year,
            created_at = recommend.created_at,
            updated_at = recommend.updated_at
        };
    }

    public static void UpdateFromRequest(this ComicRecommend recommend, UpdateComicRecommendRequest request)
    {
        if (request.rcm_count >= 0)
        {
            recommend.rcm_count = request.rcm_count;
        }

        if (request.month.HasValue)
        {
            recommend.month = request.month.Value;
        }

        if (request.year.HasValue)
        {
            recommend.year = request.year.Value;
        }
    }
}
