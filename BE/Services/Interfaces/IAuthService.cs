namespace TruyenCV.Services
{
	public interface IAuthService
	{
		Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Models.User user);
		Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);
		Task<bool> RevokeRefreshTokenAsync(string refreshToken);
		Task<bool> RevokeAllUserTokensAsync(long userId, string? exceptRefreshToken = null);
		Task<List<string>> GetUserRolesAsync(long userId);
        Task<List<string>> GetUserPermissionsAsync(long userId);
	}
}