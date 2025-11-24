using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserComicReadHistory ToEntity(this UpsertUserComicReadHistoryRequest request, long userId)
    {
        return new UserComicReadHistory
        {
            user_id = userId,
            comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
            chapter_id = request.chapter_id.ToSnowflakeId(nameof(request.chapter_id))
        };
    }

    public static void UpdateFromRequest(this UserComicReadHistory entity, UpsertUserComicReadHistoryRequest request)
    {
        entity.chapter_id = request.chapter_id.ToSnowflakeId(nameof(request.chapter_id));
    }

    public static UserComicReadHistoryResponse ToRespDTO(this UserComicReadHistory entity)
    {
        return new UserComicReadHistoryResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            comic_id = entity.comic_id.ToString(),
            chapter_id = entity.chapter_id.ToString(),
            comic_name = entity.Comic?.name,
            chapter_number = entity.ComicChapter?.chapter,
            read_at = entity.read_at,
            updated_at = entity.updated_at
        };
    }
}
