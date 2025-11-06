using System.Linq;
using TruyenCV.Models;
using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDataContext _context;

        public AuthService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(RefreshToken refreshToken)
        {
            // Sinh Access Token
            var roleNames = (refreshToken.User.Roles ?? Enumerable.Empty<UserHasRole>())
                .Where(role => role.deleted_at == null && role.revoked_at < DateTime.UtcNow)
                .Select(role => role.role_name);

            var permissionValues = (refreshToken.User.Permissions ?? Enumerable.Empty<UserHasPermission>())
                .Where(permission => permission.deleted_at == null && permission.revoked_at < DateTime.UtcNow)
                .Select(permission => permission.permissions);

            var accessToken = JwtHelper.GenerateAccessToken(refreshToken.User, roleNames, permissionValues);

            // Sinh Refresh Token
            refreshToken.expires_at = DateTime.UtcNow.AddDays(JwtHelper.RefreshTokenExpiryDays);

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            return (accessToken, refreshToken.token);
        }
        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
        {

            // Sinh Access Token
            var roleNames = (user.Roles ?? Enumerable.Empty<UserHasRole>())
                .Where(role => role.deleted_at == null && role.revoked_at < DateTime.UtcNow)
                .Select(role => role.role_name);

            var permissionValues = (user.Permissions ?? Enumerable.Empty<UserHasPermission>())
                .Where(permission => permission.deleted_at == null && permission.revoked_at < DateTime.UtcNow)
                .Select(permission => permission.permissions);

            var accessToken = JwtHelper.GenerateAccessToken(user, roleNames, permissionValues);

            // Sinh Refresh Token
            var refreshTokenValue = JwtHelper.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                token = refreshTokenValue,
                user_id = user.id,
                expires_at = DateTime.UtcNow.AddDays(JwtHelper.RefreshTokenExpiryDays),
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return (accessToken, refreshTokenValue);
        }

        public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshTokenValue)
        {
            var refreshToken = await _context.RefreshTokens
                .AsSplitQuery()
                .Where(rt => rt.token == refreshTokenValue)
                .Include(rt => rt.User)
                .Where(rt => rt.User.deleted_at == null && rt.User.is_banned == false)
                .Include(rt => rt.User.Roles.Where(role => role.deleted_at == null && (role.revoked_at == null || role.revoked_at < DateTime.UtcNow)))
                .Include(rt => rt.User.Permissions.Where(permission => permission.deleted_at == null && (permission.revoked_at == null || permission.revoked_at < DateTime.UtcNow)))
                .FirstOrDefaultAsync();

            if (refreshToken == null || !refreshToken.is_active)
                return null;

            // Lấy roles của user từ database (có thể từ bảng UserRoles hoặc trực tiếp từ User)
            // var roles = await GetUserRolesAsync(refreshToken.user_id);

            // Tạo token mới
            var newTokens = await GenerateTokensAsync(refreshToken);

            // Revoke refresh token cũ
            // refreshToken.revoked_at = DateTime.UtcNow;
            // await _context.SaveChangesAsync();

            return newTokens;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshTokenValue)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.token == refreshTokenValue);

            if (refreshToken == null || refreshToken.is_revoked)
                return false;

            refreshToken.revoked_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RevokeAllUserTokensAsync(long userId, string? exceptRefreshToken = null)
        {
            var query = _context.RefreshTokens
                .Where(rt => rt.user_id == userId && rt.revoked_at == null);

            if (!string.IsNullOrWhiteSpace(exceptRefreshToken))
            {
                query = query.Where(rt => rt.token != exceptRefreshToken);
            }

            var userTokens = await query.ToListAsync();

            foreach (var token in userTokens)
            {
                token.revoked_at = DateTime.UtcNow;
            }

            if (userTokens.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<List<string>> GetUserRolesAsync(long userId)
        {
            return await _context.UserHasRoles
                .Where(role => role.user_id == userId && role.deleted_at == null && (role.revoked_at == null || role.revoked_at < DateTime.UtcNow))
                .Select(role => role.role_name.ToString())
                .ToListAsync();
        }

        public async Task<List<string>> GetUserPermissionsAsync(long userId)
        {
            return await _context.UserHasPermissions
                .Where(permission => permission.user_id == userId && permission.deleted_at == null && (permission.revoked_at == null || permission.revoked_at < DateTime.UtcNow))
                .Select(permission => permission.permissions.ToString())
                .ToListAsync();
        }
    }
}