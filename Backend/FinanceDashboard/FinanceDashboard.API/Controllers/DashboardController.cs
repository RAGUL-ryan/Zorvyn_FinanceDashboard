using FinanceDashboard.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.API.Controllers;

/// <summary>
/// Aggregated analytics and dashboard summary endpoints.
/// Accessible by all authenticated roles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
        => _dashboardService = dashboardService;

    /// <summary>
    /// Get full dashboard summary: income, expenses, net balance,
    /// category totals, recent activity and monthly trends.
    /// [Admin, Analyst, Viewer]
    /// </summary>
    /// <param name="from">Optional start date filter</param>
    /// <param name="to">Optional end date filter</param>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to   = null)
    {
        var result = await _dashboardService.GetSummaryAsync(from, to);
        return Ok(result);
    }

    /// <summary>
    /// Get monthly income vs expense trends.
    /// [Admin, Analyst]
    /// </summary>
    /// <param name="months">How many past months to include (1–24, default 6)</param>
    [HttpGet("trends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrends([FromQuery] int months = 6)
    {
        var result = await _dashboardService.GetMonthlyTrendsAsync(months);
        return Ok(result);
    }

    /// <summary>
    /// Get spending breakdown by category with percentage share.
    /// [Admin, Analyst]
    /// </summary>
    /// <param name="from">Optional start date</param>
    /// <param name="to">Optional end date</param>
    [HttpGet("categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryBreakdown(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to   = null)
    {
        var result = await _dashboardService.GetCategoryBreakdownAsync(from, to);
        return Ok(result);
    }
}
