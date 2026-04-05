using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using FinanceDashboard.Business.DTOs;

namespace FinanceDashboard.API.Filters;

/// <summary>
/// Restricts access to users with one of the specified roles.
/// Usage: [RequireRole("Admin")]  or  [RequireRole("Admin","Analyst")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;

    public RequireRoleAttribute(params string[] roles) => _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(
                ApiResponse.Fail("Authentication required"));
            return;
        }

        var userRole = user.FindFirstValue(ClaimTypes.Role);

        if (userRole == null || !_roles.Any(r => r.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
        {
            context.Result = new ObjectResult(
                ApiResponse.Fail($"Access denied. Required role(s): {string.Join(", ", _roles)}"))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
