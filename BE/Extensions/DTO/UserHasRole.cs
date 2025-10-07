using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;
using TruyenCV.Models;

namespace TruyenCV;

public static partial class Extensions
{
    // Convert CreateUserHasRoleRequest to UserHasRole entity
    public static UserHasRole ToEntity(this CreateUserHasRoleRequest request)
    {
        return new UserHasRole
        {
            role_name = request.role_name,
            user_id = request.user_id,
            assigned_by = request.assigned_by,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };
    }

    // Convert UserHasRole entity to UserHasRoleResponse
    public static UserHasRoleResponse ToRespDTO(this UserHasRole userHasRole)
    {
        return new UserHasRoleResponse
        {
            id = userHasRole.id,
            role_name = userHasRole.role_name,
            user_id = userHasRole.user_id,
            assigned_by = userHasRole.assigned_by,
            created_at = userHasRole.created_at,
            updated_at = userHasRole.updated_at
        };
    }

    // Update UserHasRole entity from UpdateUserHasRoleRequest
    public static void UpdateFromRequest(this UserHasRole userHasRole, UpdateUserHasRoleRequest request)
    {
        userHasRole.role_name = request.role_name;
        userHasRole.user_id = request.user_id;
        userHasRole.assigned_by = request.assigned_by;
        userHasRole.updated_at = DateTime.UtcNow;
    }
}
