using System.Security.Claims;

namespace FinanceDashboard.API.Filters;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("userId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(claim?.Value, out var id) ? id : 0;
    }

    public static string GetUserRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public static string GetUserEmail(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
}
