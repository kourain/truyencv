using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Linq;
using TruyenCV.DTOs.Request;
using TruyenCV.Helpers;
using TruyenCV.Models;

namespace TruyenCV.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly ILogger<FirebaseAuthService> _logger;
        private readonly AppDataContext _context;
        private readonly IDistributedCache _redisCache;
        private readonly IUserHasRoleService _userHasRoleService;
        private readonly IUserService _userService;

        public FirebaseAuthService(
            ILogger<FirebaseAuthService> logger,
            AppDataContext context,
            IDistributedCache redisCache,
            IUserHasRoleService userHasRoleService,
            IUserService userService)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _userHasRoleService = userHasRoleService;
            _userService = userService;
        }

        public async Task<User> SignInWithFirebaseAsync(FirebaseLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.id_token))
            {
                throw new UserRequestException("Firebase ID token không hợp lệ", nameof(request.id_token));
            }

            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogWarning("Firebase Admin SDK chưa được khởi tạo.");
                throw new UserRequestException("Firebase chưa được cấu hình", nameof(request.id_token), 500);
            }

            FirebaseToken decodedToken;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.id_token);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Không thể xác thực Firebase ID token");
                throw new UserRequestException("Không thể xác thực thông tin đăng nhập Firebase", nameof(request.id_token), 401);
            }

            var email = TryGetStringClaim(decodedToken, "email");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new UserRequestException("Tài khoản Firebase chưa có email hợp lệ", nameof(request.id_token));
            }

            var now = DateTime.UtcNow;
            var user = await _context.Users.AsTracking().FirstOrDefaultAsync(u => u.email == email);

            if (user is null)
            {
                user = new User
                {
                    firebase_uid = decodedToken.Uid,
                    name = ResolveDisplayName(decodedToken, request, email),
                    email = email,
                    phone = NormalizePhone(request.phone ?? TryGetStringClaim(decodedToken, "phone_number"), allowFallback: true),
                    password = Bcrypt.HashPassword(Guid.NewGuid().ToString("N")),
                    avatar = ResolveAvatar(decodedToken, request),
                    email_verified_at = ResolveEmailVerified(decodedToken) ? now : null,
                    created_at = now,
                    updated_at = now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await _userHasRoleService.CreateUserHasRoleAsync(new CreateUserHasRoleRequest
                {
                    role_name = Roles.User,
                    user_id = user.id.ToString(),
                    assigned_by = SystemUser.id.ToString()
                });

                await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
                await _redisCache.RemoveAsync($"User:one:email:{email}");
            }
            else
            {
                if (user.deleted_at != null || user.is_banned)
                {
                    throw new UserRequestException("Tài khoản đã bị vô hiệu hóa", nameof(request.id_token), 403);
                }

                var shouldUpdate = false;
                var displayName = ResolveDisplayName(decodedToken, request, email);
                if (!string.IsNullOrWhiteSpace(displayName) && !string.Equals(user.name, displayName, StringComparison.Ordinal))
                {
                    user.name = displayName;
                    shouldUpdate = true;
                }

                var avatar = ResolveAvatar(decodedToken, request);
                if (!string.Equals(user.avatar, avatar, StringComparison.Ordinal))
                {
                    user.avatar = avatar;
                    shouldUpdate = true;
                }

                var phone = NormalizePhone(request.phone ?? TryGetStringClaim(decodedToken, "phone_number"), allowFallback: false);
                if (!string.IsNullOrWhiteSpace(phone) && !string.Equals(user.phone, phone, StringComparison.Ordinal))
                {
                    user.phone = phone;
                    shouldUpdate = true;
                }

                if (user.email_verified_at == null && ResolveEmailVerified(decodedToken))
                {
                    user.email_verified_at = now;
                    shouldUpdate = true;
                }

                if (shouldUpdate)
                {
                    user.updated_at = now;
                    await _context.SaveChangesAsync();
                    await _redisCache.AddOrUpdateInRedisAsync(user, user.id);
                    await _redisCache.RemoveAsync($"User:one:email:{email}");
                }
            }

            var enrichedUser = await _userService.GetActiveUserWithAccessAsync(user.id);
            if (enrichedUser == null)
            {
                throw new UserRequestException("Không thể tải thông tin phiên đăng nhập", nameof(request.id_token), 500);
            }

            return enrichedUser;
        }

        private static string ResolveDisplayName(FirebaseToken token, FirebaseLoginRequest request, string fallbackEmail)
        {
            if (!string.IsNullOrWhiteSpace(request.display_name))
            {
                return request.display_name.Trim();
            }

            var claimName = TryGetStringClaim(token, "name");
            if (!string.IsNullOrWhiteSpace(claimName))
            {
                return claimName;
            }

            var username = fallbackEmail.Split('@').FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(username))
            {
                return username;
            }

            return $"user_{SnowflakeIdGenerator.NextId()}";
        }

        private static string ResolveAvatar(FirebaseToken token, FirebaseLoginRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.avatar_url))
            {
                return request.avatar_url.Trim();
            }

            var picture = TryGetStringClaim(token, "picture");
            return string.IsNullOrWhiteSpace(picture) ? "default_avatar.png" : picture;
        }

        private static bool ResolveEmailVerified(FirebaseToken token)
        {
            if (token.Claims.TryGetValue("email_verified", out var claim) && claim is bool verified)
            {
                return verified;
            }

            return false;
        }

        private static string NormalizePhone(string? phone, bool allowFallback)
        {
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var digits = new string(phone.Where(char.IsDigit).ToArray());
                if (!string.IsNullOrWhiteSpace(digits))
                {
                    if (digits.Length > 15)
                    {
                        return digits[^15..];
                    }

                    if (digits.Length >= 10)
                    {
                        return digits;
                    }
                }
            }

            if (!allowFallback)
            {
                return string.Empty;
            }

            var generated = SnowflakeIdGenerator.NextId().ToString();
            if (!generated.StartsWith("0", StringComparison.Ordinal))
            {
                generated = $"0{generated}";
            }

            return generated.Length > 15 ? generated[..15] : generated.PadRight(10, '0');
        }

        private static string? TryGetStringClaim(FirebaseToken token, string claimKey)
            => token.Claims.TryGetValue(claimKey, out var value) ? value?.ToString() : null;
    }
}
