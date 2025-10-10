namespace TruyenCV.DTO.Request;

// Request DTOs for creating and updating user roles
public class CreateUserHasRoleRequest
{
	public required string role_name { get; set; }
	public required ulong user_id { get; set; }
	public required ulong assigned_by { get; set; }
}

public class UpdateUserHasRoleRequest
{
	public required ulong id { get; set; }
	public required string role_name { get; set; }
	public required ulong user_id { get; set; }
	public required ulong assigned_by { get; set; }
}
