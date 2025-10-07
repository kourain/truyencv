using TruyenCV.Models;
using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;

namespace TruyenCV.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(long userId, List<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["ValidIssuer"]!;
            var audience = jwtSettings["ValidAudience"]!;
            var accessTokenExpiry = int.Parse(jwtSettings["AccessTokenExpiryMinutes"]!);
            var refreshTokenExpiry = int.Parse(jwtSettings["RefreshTokenExpiryDays"]!);

            // Sinh Access Token
            var accessToken = JwtHelper.GenerateAccessToken(userId, roles, secretKey, issuer, audience, accessTokenExpiry);

            // Sinh Refresh Token
            var refreshTokenValue = JwtHelper.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                token = refreshTokenValue,
                user_id = userId,
                expires_at = DateTime.UtcNow.AddDays(refreshTokenExpiry),
                created_at = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return (accessToken, refreshTokenValue);
        }

        public async Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshTokenValue)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.token == refreshTokenValue);

            if (refreshToken == null || !refreshToken.is_active)
                return null;

            // Lấy roles của user từ database (có thể từ bảng UserRoles hoặc trực tiếp từ User)
            var roles = await GetUserRolesAsync(refreshToken.user_id);

            // Tạo token mới
            var newTokens = await GenerateTokensAsync(refreshToken.user_id, roles);

            // Revoke refresh token cũ
            refreshToken.revoked_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();

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

        public async Task<bool> RevokeAllUserTokensAsync(long userId)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.user_id == userId && rt.revoked_at == null)
                .ToListAsync();

            foreach (var token in userTokens)
            {
                token.revoked_at = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetUserRolesAsync(long userId)
        {
            // Tạm thời hardcode roles, sau này có thể lấy từ database
            // Có thể tạo bảng UserRoles hoặc thêm role_id vào bảng Users
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return new List<string>();

            // Logic xác định role dựa trên userId hoặc từ database
            // Ví dụ: Admin có userId = 1, còn lại là User
            if (userId == 1)
                return new List<string> { "Admin", "User" };
            else
                return new List<string> { "User" };
        }
    }
}