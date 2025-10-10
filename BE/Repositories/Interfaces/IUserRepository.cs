using TruyenCV.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho User entity
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Lấy user theo email
    /// </summary>
    /// <param name="email">Email của user</param>
    /// <returns>User nếu tìm thấy, null nếu không tìm thấy</returns>
    Task<User?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Lấy user theo id
    /// </summary>
    /// <param name="id">ID của user</param>
    /// <returns>User nếu tìm thấy, null nếu không tìm thấy</returns>
    Task<User?> GetByIdAsync(ulong id);
}