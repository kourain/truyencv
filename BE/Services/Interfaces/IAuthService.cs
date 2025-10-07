namespace TruyenCV.Services
{
	public interface IAuthService
	{
		Task<(string accessToken, string refreshToken)> GenerateTokensAsync(long userId, List<string> roles);
		Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);
		Task<bool> RevokeRefreshTokenAsync(string refreshToken);
		Task<bool> RevokeAllUserTokensAsync(long userId);
		Task<List<string>> GetUserRolesAsync(long userId);
	}
}