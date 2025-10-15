namespace TruyenCV.DTOs.Request;

public class CreateUserHasPermissionRequest
{
    public required Permissions permissions { get; set; }
    public required string user_id { get; set; }
    public required string assigned_by { get; set; }
}

public class UpdateUserHasPermissionRequest
{
    public required string id { get; set; }
    public required Permissions permissions { get; set; }
    public required string user_id { get; set; }
    public required string assigned_by { get; set; }
}
