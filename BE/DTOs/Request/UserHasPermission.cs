namespace TruyenCV.DTO.Request;

public class CreateUserHasPermissionRequest
{
    public required Permissions permissions { get; set; }
    public required long user_id { get; set; }
    public required long assigned_by { get; set; }
}

public class UpdateUserHasPermissionRequest
{
    public required long id { get; set; }
    public required Permissions permissions { get; set; }
    public required long user_id { get; set; }
    public required long assigned_by { get; set; }
}
