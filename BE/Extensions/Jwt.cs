using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static partial class Extensions
{
    public static long? GetUserId(this ClaimsPrincipal User)
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
        {
            return null;
        }

        return long.TryParse(claim.Value, out var userId) ? userId : null;
    }
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal User)
    {
        return User.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }
    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal User)
    {
        return User.FindAll("Permissions").Select(c => c.Value);
    }
    public static string GetRefreshTokenId(this ClaimsPrincipal User)
    {
        return User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value ?? string.Empty;
    }
}