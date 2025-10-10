using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserHasPermission Service
/// </summary>
public interface IUserHasPermissionService
{
    Task<UserHasPermissionResponse?> GetUserHasPermissionByIdAsync(ulong id);
    Task<IEnumerable<UserHasPermissionResponse>> GetPermissionsByUserIdAsync(ulong userId);
    Task<IEnumerable<UserHasPermissionResponse>> GetUsersByPermissionAsync(Permissions permission);
    Task<IEnumerable<UserHasPermissionResponse>> GetUserHasPermissionsAsync(int offset, int limit);
    Task<UserHasPermissionResponse> CreateUserHasPermissionAsync(CreateUserHasPermissionRequest request);
    Task<UserHasPermissionResponse?> UpdateUserHasPermissionAsync(ulong id, UpdateUserHasPermissionRequest request);
    Task<bool> DeleteUserHasPermissionAsync(ulong id);
}
