namespace TruyenCV.DTOs.Response;

public class UserHasPermissionResponse
{
    public string id { get; set; }
    public Permissions permissions { get; set; }
    public string user_id { get; set; }
    public string assigned_by { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public DateTime? revoked_at { get; set; }
    public DateTime? revoke_until { get; set; }
    public bool is_active { get; set; }
}
