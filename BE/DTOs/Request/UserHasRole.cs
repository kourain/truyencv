namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating user roles
public class CreateUserHasRoleRequest
{
	public required string role_name { get; set; }
	public required string user_id { get; set; }
	public required string assigned_by { get; set; }
}

public class UpdateUserHasRoleRequest
{
	public required string id { get; set; }
	public required string role_name { get; set; }
	public required string user_id { get; set; }
	public required string assigned_by { get; set; }
}
