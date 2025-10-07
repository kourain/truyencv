namespace TruyenCV.DTO.Request;

public class LoginRequest
{
	public required string email { get; set; }
	public required string password { get; set; }
}

public class RefreshTokenRequest
{
	public required string refresh_token { get; set; }
}