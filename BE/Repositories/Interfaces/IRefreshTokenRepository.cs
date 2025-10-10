using TruyenCV.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace TruyenCV.Repositories;

/// <summary>
/// Interface repository cho RefreshToken entity
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Lấy refresh token theo token string
    /// </summary>
    /// <param name="token">Token string</param>
    /// <returns>RefreshToken nếu tìm thấy, null nếu không tìm thấy</returns>
    Task<RefreshToken?> GetByTokenAsync(string token);
    
    /// <summary>
    /// Lấy refresh token theo id
    /// </summary>
    /// <param name="id">ID của refresh token</param>
    /// <returns>RefreshToken nếu tìm thấy, null nếu không tìm thấy</returns>
    Task<RefreshToken?> GetByIdAsync(ulong id);
    
    /// <summary>
    /// Lấy tất cả refresh token của user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>Danh sách refresh token</returns>
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(ulong userId);
    
    /// <summary>
    /// Vô hiệu hóa refresh token
    /// </summary>
    /// <param name="token">Token string</param>
    /// <returns>True nếu thành công, False nếu không tìm thấy token</returns>
    Task<bool> RevokeTokenAsync(string token);
    
    /// <summary>
    /// Vô hiệu hóa tất cả refresh token của user
    /// </summary>
    /// <param name="userId">ID của user</param>
    /// <returns>Số lượng token đã bị vô hiệu hóa</returns>
    Task<int> RevokeAllUserTokensAsync(ulong userId);
}