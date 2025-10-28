namespace TruyenCV.DTOs.Request;

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

public class FirebaseLoginRequest
{
	public required string id_token { get; set; }
	public string? display_name { get; set; }
	public string? avatar_url { get; set; }
	public string? phone { get; set; }
}