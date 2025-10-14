using System;
using System.Linq;
namespace TruyenCV;

public static partial class Extensions
{
    public static Models.User ToEntity(this DTO.Request.CreateUserRequest user)
    {
        return new Models.User
        {
            name = user.name,
            email = user.email,
            phone = user.phone,
            created_at = DateTime.UtcNow,
        };
    }
    public static DTO.Response.UserResponse ToRespDTO(this Models.User user)
    {
        return new DTO.Response.UserResponse
        {
            id = user.id,
            name = user.name,
            FullName = user.name,
            email = user.email,
            created_at = user.created_at
        };
    }
    public static DTO.Response.UserProfileResponse ToProfileDTO(this Models.User user)
    {
        return new DTO.Response.UserProfileResponse
        {
            id = user.id,
            name = user.name,
            email = user.email,
            phone = user.phone,
            avatar = user.avatar,
            created_at = user.created_at,
            updated_at = user.updated_at,
            email_verified_at = user.email_verified_at,
            banned_at = user.banned_at,
            is_banned = user.is_banned,
            read_comic_count = user.read_comic_count,
            read_chapter_count = user.read_chapter_count,
            bookmark_count = user.bookmark_count,
            coin = user.coin,
            roles = (user.Roles ?? Enumerable.Empty<Models.UserHasRole>())
                .Where(role => role.deleted_at == null)
                .Select(role => role.role_name)
                .ToArray(),
            permissions = (user.Permissions ?? Enumerable.Empty<Models.UserHasPermission>())
                .Where(permission => permission.deleted_at == null)
                .Select(permission => permission.permissions.ToString())
                .ToArray()
        };
    }
    public static Models.User UpdateFromRequest(this Models.User user, DTO.Request.UpdateUserRequest request)
    {
        user.name = request.name;
        user.email = request.email;
        user.phone = request.phone;
        user.updated_at = DateTime.UtcNow;
        return user;
    }
}
