namespace TruyenCV.DTO.Response;

public class UserHasRoleResponse
{
	public ulong id { get; set; }
	public string role_name { get; set; }
	public ulong user_id { get; set; }
	public ulong assigned_by { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
