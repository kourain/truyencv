namespace TruyenCV.Services;

public interface IPasswordResetService
{
    Task RequestPasswordResetAsync(string email, string? fullName, CancellationToken cancellationToken = default);

    Task<bool> ValidateOtpAsync(string email, string otp, CancellationToken cancellationToken = default);

    Task InvalidateOtpAsync(string email, CancellationToken cancellationToken = default);
}
