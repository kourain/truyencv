using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static partial class Extensions
{
    public static ulong? GetUserId(this ClaimsPrincipal User)
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        {
            return null;
        }

        return ulong.TryParse(claim.Value, out var userId) ? userId : null;
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