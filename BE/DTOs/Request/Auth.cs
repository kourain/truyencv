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

public class RegisterRequest
{
	public required string name { get; set; }
	public required string user_name { get; set; }
	public required string email { get; set; }
	public required string password { get; set; }
	public required string phone { get; set; }
}