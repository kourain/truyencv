using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static partial class Extensions
{
    public static long? GetUserId(this ClaimsPrincipal User)
    {
        return User.FindFirst(JwtRegisteredClaimNames.Sub).Value is string userIdStr && string.IsNullOrEmpty(userIdStr) && long.TryParse(userIdStr, out long userId) ? userId : null;
    }
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal User)
    {
        return User.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal User)
    {
        return User.FindAll("Permissions").Select(c => c.Value);
    }
}