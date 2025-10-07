namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating user roles
public class CreateUserHasRoleRequest
{
	public required string role_name { get; set; }
	public required long user_id { get; set; }
	public required long assigned_by { get; set; }
}

public class UpdateUserHasRoleRequest
{
	public required long id { get; set; }
	public required string role_name { get; set; }
	public required long user_id { get; set; }
	public required long assigned_by { get; set; }
}
