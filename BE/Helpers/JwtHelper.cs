using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TruyenCV.Helpers
{
    public static class JwtHelper
    {
        public static int AccessTokenExpiryMinutes { get; set; } = 10;
        public static int RefreshTokenExpiryDays { get; set; } = 30;
        public static string secretKey { get; set; } = "your_secret_key";
        public static string issuer { get; set; } = "your_issuer";
        public static string audience { get; set; } = "your_audience";
        public static void Init(string? secretKey = null, string? issuer = null, string? audience = null, int? accessTokenExpiry = null, int? refreshTokenExpiry = null)
        {
            if (secretKey != null)
            {
                JwtHelper.secretKey = secretKey;
            }

            if (issuer != null)
            {
                JwtHelper.issuer = issuer;
            }

            if (audience != null)
            {
                JwtHelper.audience = audience;
            }

            if (accessTokenExpiry != null)
            {
                JwtHelper.AccessTokenExpiryMinutes = accessTokenExpiry.Value;
            }

            if (refreshTokenExpiry != null)
            {
                JwtHelper.RefreshTokenExpiryDays = refreshTokenExpiry.Value;
            }
        }

        public static string GenerateAccessToken(
            Models.User user,
            IEnumerable<string> roles,
            IEnumerable<Permissions> permissions,
            string? secretKey = null,
            string? issuer = null,
            string? audience = null,
            int? expireMinutes = null)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, audience ?? JwtHelper.audience),
                new Claim(JwtRegisteredClaimNames.Iss, issuer ?? JwtHelper.issuer),
                new Claim(JwtRegisteredClaimNames.Name, user.name),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim("avatar", user.avatar)
            };

            // Thêm nhiều role nếu có
            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // Thêm permissions nếu có
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permissions", permission.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? JwtHelper.secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes ?? AccessTokenExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static ClaimsPrincipal? ValidateToken(string token, string? secretKey = null, string? issuer = null, string? audience = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey ?? JwtHelper.secretKey);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer ?? JwtHelper.issuer,
                    ValidateAudience = true,
                    ValidAudience = audience ?? JwtHelper.audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
