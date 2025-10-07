namespace TruyenCV;

public static partial class Extensions
{
	public static Models.User ToEntity(this DTO.Request.CreateUserRequest user)
	{
		return new Models.User
		{
			name = user.name,
			email = user.email,
			created_at = DateTime.UtcNow,
		};
	}
	public static DTO.Response.UserResponse ToRespDTO(this Models.User user)
	{
		return new DTO.Response.UserResponse
		{
			id = user.id,
			name = user.name,
			email = user.email,
			created_at = user.created_at
		};
	}
	public static Models.User UpdateFromRequest(this Models.User user, DTO.Request.UpdateUserRequest request)
	{
		user.name = request.user_name;
		user.email = request.email;
		user.updated_at = DateTime.UtcNow;
		return user;
	}
}
