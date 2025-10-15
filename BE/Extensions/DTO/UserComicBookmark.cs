using TruyenCV.DTOs.Request;
using TruyenCV.DTOs.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserComicBookmark ToEntity(this CreateUserComicBookmarkRequest request, long userId)
    {
        return new UserComicBookmark
        {
            user_id = userId,
            comic_id = request.comic_id.ToSnowflakeId(nameof(request.comic_id)),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static UserComicBookmarkResponse ToRespDTO(this UserComicBookmark bookmark)
    {
        return new UserComicBookmarkResponse
        {
            id = bookmark._id,
            user_id = bookmark.user_id.ToString(),
            comic_id = bookmark.comic_id.ToString(),
            created_at = bookmark.created_at,
            updated_at = bookmark.updated_at
        };
    }
}
