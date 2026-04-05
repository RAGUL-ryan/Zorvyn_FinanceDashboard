using System.Globalization;
using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Dashboard;
using FinanceDashboard.Business.Services.Interfaces;
using FinanceDashboard.Data.Repositories.Interfaces;

namespace FinanceDashboard.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly ITransactionRepository _repo;

    public DashboardService(ITransactionRepository repo) => _repo = repo;

    public async Task<ApiResponse<DashboardSummaryDto>> GetSummaryAsync(DateTime? from = null, DateTime? to = null)
    {
        var totalIncome   = await _repo.GetTotalByTypeAsync("income",  from, to);
        var totalExpenses = await _repo.GetTotalByTypeAsync("expense", from, to);
        var categoryTotals = await _repo.GetTotalsByCategoryAsync(from, to);
        var recent        = await _repo.GetRecentAsync(10);
        var trends        = await _repo.GetMonthlyTrendsAsync(6);

        var summary = new DashboardSummaryDto
        {
            TotalIncome    = totalIncome,
            TotalExpenses  = totalExpenses,
            NetBalance     = totalIncome - totalExpenses,
            TotalTransactions = categoryTotals.Values.Count,
            CategoryTotals = categoryTotals,
            RecentActivity = recent.Select(t => new RecentTransactionDto
            {
                Id          = t.Id,
                Amount      = t.Amount,
                Type        = t.Type,
                Category    = t.Category,
                Date        = t.Date,
                Description = t.Description
            }),
            MonthlyTrends = trends.Select(t => new MonthlyTrendDto
            {
                Year      = t.Year,
                Month     = t.Month,
                MonthName = new DateTime(t.Year, t.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                Income    = t.Income,
                Expenses  = t.Expense,
                Net       = t.Income - t.Expense
            })
        };

        return ApiResponse<DashboardSummaryDto>.Ok(summary);
    }

    public async Task<ApiResponse<IEnumerable<MonthlyTrendDto>>> GetMonthlyTrendsAsync(int months = 6)
    {
        if (months < 1 || months > 24) months = 6;
        var trends = await _repo.GetMonthlyTrendsAsync(months);

        var result = trends.Select(t => new MonthlyTrendDto
        {
            Year      = t.Year,
            Month     = t.Month,
            MonthName = new DateTime(t.Year, t.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
            Income    = t.Income,
            Expenses  = t.Expense,
            Net       = t.Income - t.Expense
        });

        return ApiResponse<IEnumerable<MonthlyTrendDto>>.Ok(result);
    }

    public async Task<ApiResponse<IEnumerable<CategorySummaryDto>>> GetCategoryBreakdownAsync(DateTime? from = null, DateTime? to = null)
    {
        var totals = await _repo.GetTotalsByCategoryAsync(from, to);
        var grandTotal = totals.Values.Sum();

        var result = totals.Select(kvp => new CategorySummaryDto
        {
            Category   = kvp.Key,
            Total      = kvp.Value,
            Percentage = grandTotal > 0 ? Math.Round(kvp.Value / grandTotal * 100, 2) : 0
        }).OrderByDescending(c => c.Total);

        return ApiResponse<IEnumerable<CategorySummaryDto>>.Ok(result);
    }
}
