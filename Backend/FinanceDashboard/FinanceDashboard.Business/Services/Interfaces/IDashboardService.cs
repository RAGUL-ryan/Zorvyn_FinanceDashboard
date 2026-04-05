using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Dashboard;

namespace FinanceDashboard.Business.Services.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(DateTime? from = null, DateTime? to = null);
    Task<ApiResponse<IEnumerable<MonthlyTrendDto>>> GetMonthlyTrendsAsync(int months = 6);
    Task<ApiResponse<IEnumerable<CategorySummaryDto>>> GetCategoryBreakdownAsync(DateTime? from = null, DateTime? to = null);
}
