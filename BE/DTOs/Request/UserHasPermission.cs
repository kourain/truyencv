namespace TruyenCV.DTO.Request;

public class CreateUserHasPermissionRequest
{
    public required Permissions permissions { get; set; }
    public required ulong user_id { get; set; }
    public required ulong assigned_by { get; set; }
}

public class UpdateUserHasPermissionRequest
{
    public required ulong id { get; set; }
    public required Permissions permissions { get; set; }
    public required ulong user_id { get; set; }
    public required ulong assigned_by { get; set; }
}
