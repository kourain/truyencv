using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TruyenCV.Services;

public class PasswordResetService : IPasswordResetService
{
    private const string OtpPrefix = "PasswordReset:otp:";
    private const string ThrottlePrefix = "PasswordReset:throttle:";
    private const string DailyCountPrefix = "PasswordReset:daily:";
    private const int DailyRequestLimit = 2;
    private const int MaxFailedAttempts = 3;
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ThrottleDuration = TimeSpan.FromHours(12);

    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PasswordResetService> _logger;

    public PasswordResetService(
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PasswordResetService> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RequestPasswordResetAsync(string email, string? fullName, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            throw new UserRequestException("Email không hợp lệ");
        }

        var throttleKey = BuildThrottleKey(normalizedEmail);
        if (await _cache.GetStringAsync(throttleKey, cancellationToken) is not null)
        {
            _logger.LogInformation("Password reset OTP request throttled for {Email}", normalizedEmail);
            throw new UserRequestException("Vui lòng chờ trước khi yêu cầu mã OTP đặt lại mật khẩu mới");
        }

        var dailyCountKey = BuildDailyCountKey(normalizedEmail);
        var currentDailyCount = await GetDailyRequestCountAsync(dailyCountKey, cancellationToken);
        if (currentDailyCount >= DailyRequestLimit)
        {
            _logger.LogInformation("Password reset daily request limit reached for {Email}", normalizedEmail);
            await _cache.SetStringAsync(
                throttleKey,
                "1",
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ThrottleDuration
                },
                cancellationToken);
            throw new UserRequestException("Vui lòng chờ trước khi yêu cầu mã OTP đặt lại mật khẩu mới");
        }

        var otp = GenerateOtp();
        var expiresAtUtc = DateTime.UtcNow.Add(OtpLifetime);
        var cacheEntry = new PasswordResetCacheEntry
        {
            otp = otp,
            expires_at = expiresAtUtc,
            failed_attempts = 0,
            full_name = fullName
        };

        var cacheValue = JsonConvert.SerializeObject(cacheEntry);
        var otpKey = BuildOtpKey(normalizedEmail);

        await _cache.SetStringAsync(
            otpKey,
            cacheValue,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = OtpLifetime
            },
            cancellationToken);

        await _cache.SetStringAsync(
            throttleKey,
            "1",
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ThrottleDuration
            },
            cancellationToken);

        try
        {
            await SendOtpEmailAsync(normalizedEmail, fullName, otp, expiresAtUtc, cancellationToken);
            await SetDailyRequestCountAsync(dailyCountKey, currentDailyCount + 1, cancellationToken);
        }
        catch
        {
            await _cache.RemoveAsync(otpKey, cancellationToken);
            await _cache.RemoveAsync(throttleKey, cancellationToken);
            throw;
        }
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            return false;
        }

        var otpKey = BuildOtpKey(normalizedEmail);
        var cacheValue = await _cache.GetStringAsync(otpKey, cancellationToken);
        if (string.IsNullOrEmpty(cacheValue))
        {
            return false;
        }

        PasswordResetCacheEntry? cacheEntry = null;
        try
        {
            cacheEntry = JsonConvert.DeserializeObject<PasswordResetCacheEntry>(cacheValue);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Không thể deserialize OTP trong cache cho {Email}", normalizedEmail);
        }

        if (cacheEntry == null)
        {
            return false;
        }

        var remainingLifetime = cacheEntry.expires_at - DateTime.UtcNow;
        if (remainingLifetime <= TimeSpan.Zero)
        {
            await _cache.RemoveAsync(otpKey, cancellationToken);
            return false;
        }

        if (string.Equals(cacheEntry.otp, otp, StringComparison.Ordinal))
        {
            return true;
        }

        cacheEntry.failed_attempts++;

        if (cacheEntry.failed_attempts >= MaxFailedAttempts)
        {
            // await _cache.RemoveAsync(otpKey, cancellationToken);
            await SendOtpFailureAlertEmailAsync(normalizedEmail, cacheEntry.full_name, cancellationToken);
        }
        else
        {
            await _cache.SetStringAsync(
                otpKey,
                JsonConvert.SerializeObject(cacheEntry),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = remainingLifetime
                },
                cancellationToken);
        }

        return false;
    }

    public async Task InvalidateOtpAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (string.IsNullOrEmpty(normalizedEmail))
        {
            return;
        }

        await _cache.RemoveAsync(BuildOtpKey(normalizedEmail), cancellationToken);
    }

    private async Task SendOtpEmailAsync(string email, string? fullName, string otp, DateTime expiresAtUtc, CancellationToken cancellationToken)
    {
        var endpoint = _configuration["MailEndpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("MailEndpoint chưa được cấu hình");
        }

        var client = _httpClientFactory.CreateClient();
        var subject = "Mã OTP đặt lại mật khẩu TruyenCV";
        var greeting = string.IsNullOrWhiteSpace(fullName) ? "Xin chào" : $"Xin chào {fullName}";
        var expiresAtLocal = TimeZoneInfo.ConvertTimeFromUtc(expiresAtUtc, GetVietnamTimeZone());
        var formattedExpiryLocal = expiresAtLocal.ToString("hh:mm tt dd/MM/yyyy 'giờ Việt Nam'");
        var formattedExpiryUtc = expiresAtUtc.ToString("HH:mm dd/MM/yyyy 'UTC'");
        var validityMinutes = (int)Math.Round(OtpLifetime.TotalMinutes);
        var htmlContent = $$"""
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <title>{{subject}}</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, sans-serif; background-color: #f5f5f5; margin: 0; padding: 24px; }
        .container { max-width: 520px; margin: 0 auto; background: #ffffff; border-radius: 16px; padding: 24px; box-shadow: 0 12px 40px rgba(87, 117, 255, 0.12); }
        .title { color: #1f2937; font-size: 20px; font-weight: 600; margin-bottom: 16px; }
        .otp { display: inline-block; padding: 16px 24px; font-size: 32px; font-weight: 700; letter-spacing: 12px; color: #ffffff; background: linear-gradient(135deg, #4f46e5, #9333ea); border-radius: 12px; margin: 24px 0; text-align: center; }
        .muted { color: #6b7280; font-size: 14px; line-height: 1.6; }
        .footer { margin-top: 32px; font-size: 12px; color: #9ca3af; text-align: center; }
    </style>
</head>
<body>
    <div class="container">
        <p class="muted">{{greeting}},</p>
        <p class="title">Mã OTP đặt lại mật khẩu của bạn</p>
        <p class="muted">Vui lòng sử dụng mã OTP bên dưới để hoàn tất việc đặt lại mật khẩu. Mã sẽ hết hạn vào {{formattedExpiryLocal}} (tương ứng {{formattedExpiryUtc}}), tức sau khoảng {{validityMinutes}} phút kể từ khi email được gửi.</p>
        <div class="otp">{{otp}}</div>
        <p class="muted">Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này hoặc liên hệ với chúng tôi để được hỗ trợ.</p>
        <p class="muted">Trân trọng,<br/>Đội ngũ TruyenCV</p>
        <div class="footer">Email này được gửi tự động, vui lòng không trả lời lại.</div>
    </div>
</body>
</html>
""";
        var payload = new
        {
            recipients = new[] { email },
            subject,
            html_content = htmlContent,
            is_html = true
        };

        using var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        using var response = await client.PostAsync(endpoint, requestContent, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Không thể gửi email OTP: {(int)response.StatusCode} - {body}");
        }
    }

    private async Task SendOtpFailureAlertEmailAsync(string email, string? fullName, CancellationToken cancellationToken)
    {
        try
        {
            var endpoint = _configuration["MailEndpoint"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                _logger.LogWarning("MailEndpoint chưa được cấu hình khi gửi cảnh báo OTP cho {Email}", email);
                return;
            }

            var client = _httpClientFactory.CreateClient();
            var subject = "Cảnh báo: OTP đặt lại mật khẩu bị nhập sai";
            var greeting = string.IsNullOrWhiteSpace(fullName) ? "Xin chào" : $"Xin chào {fullName}";
            var htmlContent = $$"""
            <!DOCTYPE html>
            <html lang="vi">
            <head>
                <meta charset="utf-8" />
                <title>{{subject}}</title>
                <style>
                    body { font-family: 'Segoe UI', Tahoma, sans-serif; background-color: #fef3c7; margin: 0; padding: 24px; }
                    .container { max-width: 520px; margin: 0 auto; background: #ffffff; border-radius: 16px; padding: 24px; box-shadow: 0 12px 40px rgba(249, 115, 22, 0.16); }
                    .title { color: #b45309; font-size: 18px; font-weight: 600; margin-bottom: 16px; }
                    .muted { color: #92400e; font-size: 14px; line-height: 1.6; }
                    .footer { margin-top: 32px; font-size: 12px; color: #b45309; text-align: center; }
                </style>
            </head>
            <body>
                <div class="container">
                    <p class="muted">{{greeting}},</p>
                    <p class="title">Chúng tôi vừa phát hiện 3 lần nhập sai mã OTP liên tiếp</p>
                    <p class="muted">Mã OTP hiện tại của bạn đã bị hủy để đảm bảo an toàn. Nếu đó không phải là bạn, hãy đổi mật khẩu ngay sau khi đăng nhập hoặc liên hệ với đội ngũ TruyenCV để được hỗ trợ.</p>
                    <p class="muted">Bạn có thể yêu cầu mã OTP mới sau ít phút nữa.</p>
                    <p class="muted">Trân trọng,<br/>Đội ngũ TruyenCV</p>
                    <div class="footer">Email cảnh báo tự động - vui lòng không trả lời.</div>
                </div>
            </body>
            </html>
            """;

            var payload = new
            {
                recipients = new[] { email },
                subject,
                html_content = htmlContent,
                is_html = true
            };

            using var requestContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            using var response = await client.PostAsync(endpoint, requestContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Không thể gửi email cảnh báo OTP cho {Email}: {Status} - {Body}", email, response.StatusCode, body);
                throw new UserRequestException("Không thể gửi email cảnh báo OTP");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi email cảnh báo OTP cho {Email}", email);
            throw new UserRequestException("Lỗi khi gửi email cảnh báo OTP");
        }
    }

    private static string BuildOtpKey(string email) => $"{OtpPrefix}{email}";

    private static string BuildThrottleKey(string email) => $"{ThrottlePrefix}{email}";

    private static string BuildDailyCountKey(string email)
    {
        var dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"{DailyCountPrefix}{dateStamp}:{email}";
    }

    private static TimeSpan GetRemainingTimeToday()
    {
        var now = DateTime.UtcNow;
        var tomorrow = now.Date.AddDays(1);
        var remaining = tomorrow - now;
        return remaining <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : remaining;
    }

    private async Task<int> GetDailyRequestCountAsync(string key, CancellationToken cancellationToken)
    {
        var value = await _cache.GetStringAsync(key, cancellationToken);
        return int.TryParse(value, out var count) ? count : 0;
    }

    private async Task SetDailyRequestCountAsync(string key, int value, CancellationToken cancellationToken)
    {
        await _cache.SetStringAsync(
            key,
            value.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = GetRemainingTimeToday()
            },
            cancellationToken);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string GenerateOtp()
    {
        Span<byte> bytes = stackalloc byte[4];
        RandomNumberGenerator.Fill(bytes);
        var numeric = BitConverter.ToUInt32(bytes);
        var otp = (numeric % 900000) + 100000;
        return otp.ToString();
    }

    private static TimeZoneInfo GetVietnamTimeZone()
    {
        const string windowsId = "SE Asia Standard Time";
        const string ianaId = "Asia/Ho_Chi_Minh";

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
        }
        catch (TimeZoneNotFoundException)
        {
        }
        catch (InvalidTimeZoneException)
        {
        }

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
        }
        catch (TimeZoneNotFoundException)
        {
        }
        catch (InvalidTimeZoneException)
        {
        }

        return TimeZoneInfo.CreateCustomTimeZone("UTC+7", TimeSpan.FromHours(7), "UTC+7", "UTC+7");
    }

    private sealed class PasswordResetCacheEntry
    {
        public string otp { get; set; } = string.Empty;
        public DateTime expires_at { get; set; }
        public int failed_attempts { get; set; }
        public string? full_name { get; set; }
    }
}
