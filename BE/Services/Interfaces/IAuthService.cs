namespace TruyenCV.Services
{
	public interface IAuthService
	{
		Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Models.User user);
		Task<(string accessToken, string refreshToken)?> RefreshTokenAsync(string refreshToken);
		Task<bool> RevokeRefreshTokenAsync(string refreshToken);
		Task<bool> RevokeAllUserTokensAsync(ulong userId, string? exceptRefreshToken = null);
		Task<List<string>> GetUserRolesAsync(ulong userId);
        Task<List<string>> GetUserPermissionsAsync(ulong userId);
	}
}