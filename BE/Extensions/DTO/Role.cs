namespace TruyenCV;

public static partial class Extensions
{
	public static Models.Role ToEntity(this DTO.Request.CreateRoleRequest role)
	{
		return new Models.Role
		{
			name = role.name,
			title = role.title,
		};
	}
	
	public static DTO.Response.RoleResponse ToRespDTO(this Models.Role role)
	{
		return new DTO.Response.RoleResponse
		{
			id = role.id,
			name = role.name,
			title = role.title,
		};
	}
	
	public static Models.Role UpdateFromRequest(this Models.Role role, DTO.Request.UpdateRoleRequest request)
	{
		role.name = request.name;
		role.title = request.title;
		return role;
	}
}
