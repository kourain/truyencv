using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserHasRole entity
/// </summary>
public interface IUserHasRoleRepository : IRepository<UserHasRole>
{
    /// <summary>
    /// Lấy user role theo id
    /// </summary>
    /// <param name="id">ID của user role</param>
    /// <returns>UserHasRole nếu tìm thấy, null nếu không tìm thấy</returns>
    Task<UserHasRole?> GetByIdAsync(ulong id);

    /// <summary>
    /// Lấy danh sách role của user theo user_id
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>Danh sách UserHasRole</returns>
    Task<IEnumerable<UserHasRole>> GetByUserIdAsync(ulong userId);

    /// <summary>
    /// Lấy danh sách user có role cụ thể
    /// </summary>
    /// <param name="roleName">Tên role</param>
    /// <returns>Danh sách UserHasRole</returns>
    Task<IEnumerable<UserHasRole>> GetByRoleNameAsync(string roleName);
}
