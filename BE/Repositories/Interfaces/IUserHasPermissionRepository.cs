using TruyenCV.Models;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho UserHasPermission entity
/// </summary>
public interface IUserHasPermissionRepository : IRepository<UserHasPermission>
{
    /// <summary>
    /// Lấy bản ghi theo id
    /// </summary>
    Task<UserHasPermission?> GetByIdAsync(long id);

    /// <summary>
    /// Lấy danh sách permission theo user
    /// </summary>
    Task<IEnumerable<UserHasPermission>> GetByUserIdAsync(long userId);

    /// <summary>
    /// Lấy danh sách user theo permission
    /// </summary>
    Task<IEnumerable<UserHasPermission>> GetByPermissionAsync(Permissions permission);

    /// <summary>
    /// Lấy bản ghi cụ thể theo user và permission
    /// </summary>
    Task<UserHasPermission?> GetByUserPermissionAsync(long userId, Permissions permission);
}
