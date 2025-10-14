using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserComicReadHistory ToEntity(this UpsertUserComicReadHistoryRequest request, long userId)
    {
        return new UserComicReadHistory
        {
            user_id = userId,
            comic_id = request.comic_id,
            chapter_id = request.chapter_id,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static void UpdateFromRequest(this UserComicReadHistory entity, UpsertUserComicReadHistoryRequest request)
    {
        entity.chapter_id = request.chapter_id;
        entity.updated_at = DateTime.UtcNow;
    }

    public static UserComicReadHistoryResponse ToRespDTO(this UserComicReadHistory entity)
    {
        return new UserComicReadHistoryResponse
        {
            id = entity.id,
            user_id = entity.user_id,
            comic_id = entity.comic_id,
            chapter_id = entity.chapter_id,
            read_at = entity.read_at,
            updated_at = entity.updated_at
        };
    }
}
