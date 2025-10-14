namespace TruyenCV.DTO.Response;

public class UserHasRoleResponse
{
	public long id { get; set; }
	public string role_name { get; set; }
	public long user_id { get; set; }
	public long assigned_by { get; set; }
	public DateTime created_at { get; set; }
	public DateTime updated_at { get; set; }
}
