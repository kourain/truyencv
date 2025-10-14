using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserHasPermission Service
/// </summary>
public interface IUserHasPermissionService
{
    Task<UserHasPermissionResponse?> GetUserHasPermissionByIdAsync(long id);
    Task<IEnumerable<UserHasPermissionResponse>> GetPermissionsByUserIdAsync(long userId);
    Task<IEnumerable<UserHasPermissionResponse>> GetUsersByPermissionAsync(Permissions permission);
    Task<IEnumerable<UserHasPermissionResponse>> GetUserHasPermissionsAsync(int offset, int limit);
    Task<UserHasPermissionResponse> CreateUserHasPermissionAsync(CreateUserHasPermissionRequest request);
    Task<UserHasPermissionResponse?> UpdateUserHasPermissionAsync(long id, UpdateUserHasPermissionRequest request);
    Task<bool> DeleteUserHasPermissionAsync(long id);
}
