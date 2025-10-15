namespace TruyenCV.DTOs.Request;
// Request DTOs for creating and updating users
public class CreateUserRequest
{
	public required string name { get; set; }
	public required string user_name { get; set; }
	public required string email { get; set; }
	public required string password { get; set; }
	public required string phone { get; set; }
}

public class UpdateUserRequest
{
	public required string id { get; set; }
	public required string user_name { get; set; }
	public required string name { get; set; }
	public required string email { get; set; }
	public required string phone { get; set; }
}

public class ChangePasswordRequest
{
	public required string current_password { get; set; }
	public required string new_password { get; set; }
}