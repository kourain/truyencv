using System;
using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserComicUnlockHistory ToEntity(this CreateUserComicUnlockHistoryRequest request)
    {
        return new UserComicUnlockHistory
        {
            user_id = request.user_id.ToSnowflakeId(nameof(request.user_id)),
            comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
            comic_chapter_id = request.comic_chapter_id.ToSnowflakeId(nameof(request.comic_chapter_id)),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static UserComicUnlockHistoryResponse ToRespDTO(this UserComicUnlockHistory entity)
    {
        return new UserComicUnlockHistoryResponse
        {
            id = entity._id,
            user_id = entity.user_id.ToString(),
            comic_id = entity.comic_id.ToString(),
            comic_chapter_id = entity.comic_chapter_id.ToString(),
            created_at = entity.created_at,
            updated_at = entity.updated_at
        };
    }
}
