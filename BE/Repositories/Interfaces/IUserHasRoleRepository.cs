using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserHasRole entity
/// </summary>
public interface IUserHasRoleRepository : IRepository<UserHasRole>
{
    /// <summary>
    /// Lấy danh sách role của user theo user_id
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>Danh sách UserHasRole</returns>
    Task<IEnumerable<UserHasRole>> GetByUserIdAsync(long userId);

    /// <summary>
    /// Lấy danh sách user có role cụ thể
    /// </summary>
    /// <param name="roleName">Tên role</param>
    /// <returns>Danh sách UserHasRole</returns>
    Task<IEnumerable<UserHasRole>> GetByRoleNameAsync(string roleName);
}
