namespace TruyenCV.DTO.Request;

public class CreateRoleRequest
{
	public required string name { get; set; }
	public required string title { get; set; }
}

public class UpdateRoleRequest
{
	public required string name { get; set; }
	public required string title { get; set; }
}
