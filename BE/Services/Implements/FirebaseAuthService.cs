using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public FirebaseAuthService(
            ILogger<FirebaseAuthService> logger,
            AppDataContext context,
            IDistributedCache redisCache,
            IUserHasRoleService userHasRoleService,
            IUserService userService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _redisCache = redisCache;
            _userHasRoleService = userHasRoleService;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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
                var tempPass = Guid.NewGuid().ToString("N").Substring(0, 16);
                user = new User
                {
                    firebase_uid = decodedToken.Uid,
                    name = ResolveDisplayName(decodedToken, request, email),
                    email = email ?? decodedToken.Uid + "@firebaseuser.local",
                    phone = NormalizePhone(request.phone ?? TryGetStringClaim(decodedToken, "phone_number"), allowFallback: true),
                    password = Bcrypt.HashPassword(tempPass),
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
                var cacheTasks = Task.WhenAll(
                    _redisCache.AddOrUpdateInRedisAsync(user, user.id),
                    _redisCache.RemoveAsync($"User:one:email:{email}")
                );

                await Task.WhenAll(cacheTasks, SendWelcomeEmailAsync(user.email, user.name, tempPass));
            }
            else
            {
                if (user.deleted_at != null || user.is_banned)
                {
                    throw new UserRequestException("Tài khoản đã bị vô hiệu hóa", nameof(request.id_token), 403);
                }

                var shouldUpdate = false;
                var displayName = ResolveDisplayName(decodedToken, request, email);
                if (!string.IsNullOrWhiteSpace(decodedToken.Uid) && !string.Equals(user.firebase_uid, decodedToken.Uid, StringComparison.Ordinal))
                {
                    user.firebase_uid = decodedToken.Uid;
                    shouldUpdate = true;
                }
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
                    await Task.WhenAll(
                        _context.SaveChangesAsync(),
                        _redisCache.AddOrUpdateInRedisAsync(user, user.id),
                        _redisCache.RemoveAsync($"User:one:email:{email}")
                    );
                }
            }

            var enrichedUser = await _userService.GetActiveUserWithAccessAsync(user.id);
            if (enrichedUser == null)
            {
                throw new UserRequestException("Không thể tải thông tin phiên đăng nhập", nameof(request.id_token), 500);
            }

            return enrichedUser;
        }

        private async Task SendWelcomeEmailAsync(string email, string fullName, string defaultPassword, CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = _configuration["MailEndpoint"];
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    _logger.LogWarning("MailEndpoint chưa được cấu hình khi gửi email chào mừng cho {Email}", email);
                    return;
                }

                var client = _httpClientFactory.CreateClient();
                var subject = "Chào mừng đến TruyenCV";
                var greeting = string.IsNullOrWhiteSpace(fullName) ? "Xin chào bạn," : $"Xin chào {fullName},";
                var htmlContent = $@"<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""utf-8"" />
    <title>{subject}</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, sans-serif; background-color: #f5f5f5; margin: 0; padding: 24px; }}
        .container {{ max-width: 520px; margin: 0 auto; background: #ffffff; border-radius: 16px; padding: 24px; box-shadow: 0 12px 40px rgba(79, 70, 229, 0.18); }}
        .title {{ color: #111827; font-size: 20px; font-weight: 600; margin-bottom: 12px; }}
        .password-box {{ display: inline-flex; align-items: center; gap: 12px; border: 1px dashed #4f46e5; padding: 14px 18px; border-radius: 12px; font-size: 18px; font-weight: 600; color: #111827; background: #eef2ff; }}
        .muted {{ color: #6b7280; font-size: 14px; line-height: 1.6; }}
        .footer {{ margin-top: 32px; font-size: 12px; color: #9ca3af; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <p class=""muted"">{greeting}</p>
        <p class=""title"">Tài khoản của bạn đã sẵn sàng</p>
        <p class=""muted"">Đây là mật khẩu tạm thời để bạn đăng nhập vào TruyenCV. Vui lòng đăng nhập và đổi mật khẩu ngay trong mục Bảo mật để đảm bảo an toàn.</p>
        <div class=""password-box"">
            <span>Mật khẩu:</span>
            <span>{defaultPassword}</span>
        </div>
        <p class=""muted"">Nếu bạn không thực hiện đăng nhập bằng Firebase, vui lòng bỏ qua email này hoặc liên hệ đội ngũ TruyenCV để được hỗ trợ.</p>
        <p class=""muted"">Trân trọng,<br/>Đội ngũ TruyenCV</p>
        <div class=""footer"">Email được gửi tự động, vui lòng không trả lời.</div>
    </div>
</body>
</html>";

                var payload = new
                {
                    recipients = new[] { email },
                    subject,
                    html_content = htmlContent,
                    is_html = true
                };

                using var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(endpoint, content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Không thể gửi email chào mừng tới {Email}: {StatusCode} - {Body}", email, response.StatusCode, body);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email chào mừng cho {Email}", email);
            }
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
