using FinanceDashboard.Data.Entities;

namespace FinanceDashboard.Data.Repositories.Interfaces;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? type = null, string? category = null,
        DateTime? from = null, DateTime? to = null,
        string? search = null);

    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<decimal> GetTotalByTypeAsync(string type, DateTime? from = null, DateTime? to = null);
    Task<Dictionary<string, decimal>> GetTotalsByCategoryAsync(DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<Transaction>> GetRecentAsync(int count = 10);
    Task<IEnumerable<(int Year, int Month, decimal Income, decimal Expense)>> GetMonthlyTrendsAsync(int months = 6);
}
