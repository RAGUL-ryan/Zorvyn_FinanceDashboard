namespace FinanceDashboard.Business.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetBalance { get; set; }
    public int TotalTransactions { get; set; }
    public Dictionary<string, decimal> CategoryTotals { get; set; } = new();
    public IEnumerable<RecentTransactionDto> RecentActivity { get; set; } = new List<RecentTransactionDto>();
    public IEnumerable<MonthlyTrendDto> MonthlyTrends { get; set; } = new List<MonthlyTrendDto>();
}

public class RecentTransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class MonthlyTrendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal Net { get; set; }
}

public class CategorySummaryDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Percentage { get; set; }
}
