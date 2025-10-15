using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    public static UserHasPermission ToEntity(this CreateUserHasPermissionRequest request)
    {
        return new UserHasPermission
        {
            permissions = request.permissions,
            user_id = request.user_id.ToSnowflakeId(nameof(request.user_id)),
            assigned_by = request.assigned_by.ToSnowflakeId(nameof(request.assigned_by)),
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    public static void UpdateFromRequest(this UserHasPermission entity, UpdateUserHasPermissionRequest request)
    {
        entity.permissions = request.permissions;
        entity.user_id = request.user_id.ToSnowflakeId(nameof(request.user_id));
        entity.assigned_by = request.assigned_by.ToSnowflakeId(nameof(request.assigned_by));
        entity.updated_at = DateTime.UtcNow;
    }

    public static UserHasPermissionResponse ToRespDTO(this UserHasPermission entity)
    {
        return new UserHasPermissionResponse
        {
            id = entity._id,
            permissions = entity.permissions,
            user_id = entity.user_id.ToString(),
            assigned_by = entity.assigned_by.ToString(),
            created_at = entity.created_at,
            updated_at = entity.updated_at,
            revoked_at = entity.revoked_at,
            revoke_until = entity.revoke_until,
            is_active = entity.is_active
        };
    }
}
