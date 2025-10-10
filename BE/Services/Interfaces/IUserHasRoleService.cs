using TruyenCV.DTO.Request;
using TruyenCV.DTO.Response;

namespace TruyenCV.Services;

/// <summary>
/// Interface cho UserHasRole Service
/// </summary>
public interface IUserHasRoleService
{
    /// <summary>
    /// Lấy thông tin user role theo ID
    /// </summary>
    /// <param name="id">ID của user role</param>
    /// <returns>Thông tin user role</returns>
    Task<UserHasRoleResponse?> GetUserHasRoleByIdAsync(ulong id);

    /// <summary>
    /// Lấy danh sách role của user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>Danh sách role của user</returns>
    Task<IEnumerable<UserHasRoleResponse>> GetRolesByUserIdAsync(ulong userId);

    /// <summary>
    /// Lấy danh sách user có role cụ thể
    /// </summary>
    /// <param name="roleName">Tên role</param>
    /// <returns>Danh sách user có role</returns>
    Task<IEnumerable<UserHasRoleResponse>> GetUsersByRoleNameAsync(string roleName);

    /// <summary>
    /// Lấy danh sách user role với phân trang
    /// </summary>
    /// <param name="offset">Vị trí bắt đầu</param>
    /// <param name="limit">Số lượng bản ghi</param>
    /// <returns>Danh sách user role</returns>
    Task<IEnumerable<UserHasRoleResponse>> GetUserHasRolesAsync(int offset, int limit);

    /// <summary>
    /// Gán role cho user
    /// </summary>
    /// <param name="request">Thông tin gán role</param>
    /// <returns>Thông tin user role đã tạo</returns>
    Task<UserHasRoleResponse> CreateUserHasRoleAsync(CreateUserHasRoleRequest request);

    /// <summary>
    /// Cập nhật thông tin user role
    /// </summary>
    /// <param name="id">ID của user role</param>
    /// <param name="request">Thông tin cập nhật</param>
    /// <returns>Thông tin user role đã cập nhật</returns>
    Task<UserHasRoleResponse?> UpdateUserHasRoleAsync(ulong id, UpdateUserHasRoleRequest request);

    /// <summary>
    /// Xóa user role
    /// </summary>
    /// <param name="id">ID của user role</param>
    /// <returns>True nếu xóa thành công, ngược lại là False</returns>
    Task<bool> DeleteUserHasRoleAsync(ulong id);
}
