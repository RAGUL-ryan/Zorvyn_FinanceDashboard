using FinanceDashboard.Data.Context;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Data.Repositories.Implementations;

public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
{
    public TransactionRepository(FinanceDbContext context) : base(context) { }

    public async Task<(IEnumerable<Transaction> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? type = null, string? category = null,
        DateTime? from = null, DateTime? to = null,
        string? search = null)
    {
        var query = _dbSet.Include(t => t.CreatedByUser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(t => t.Type.ToLower() == type.ToLower());

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(t => t.Category.ToLower() == category.ToLower());

        if (from.HasValue)
            query = query.Where(t => t.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.Date <= to.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t =>
                t.Description.Contains(search) ||
                t.Category.Contains(search) ||
                (t.Notes != null && t.Notes.Contains(search)));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _dbSet.Where(t => t.Date >= from && t.Date <= to)
                       .OrderByDescending(t => t.Date)
                       .ToListAsync();

    public async Task<decimal> GetTotalByTypeAsync(string type, DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.Where(t => t.Type.ToLower() == type.ToLower());
        if (from.HasValue) query = query.Where(t => t.Date >= from.Value);
        if (to.HasValue)   query = query.Where(t => t.Date <= to.Value);
        return await query.SumAsync(t => t.Amount);
    }

    public async Task<Dictionary<string, decimal>> GetTotalsByCategoryAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _dbSet.AsQueryable();
        if (from.HasValue) query = query.Where(t => t.Date >= from.Value);
        if (to.HasValue)   query = query.Where(t => t.Date <= to.Value);

        return await query
            .GroupBy(t => t.Category)
            .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
            .ToDictionaryAsync(x => x.Category, x => x.Total);
    }

    public async Task<IEnumerable<Transaction>> GetRecentAsync(int count = 10)
        => await _dbSet.Include(t => t.CreatedByUser)
                       .OrderByDescending(t => t.Date)
                       .Take(count)
                       .ToListAsync();

    public async Task<IEnumerable<(int Year, int Month, decimal Income, decimal Expense)>> GetMonthlyTrendsAsync(int months = 6)
    {
        var from = DateTime.UtcNow.AddMonths(-months);

        var data = await _dbSet
            .Where(t => t.Date >= from)
            .GroupBy(t => new { t.Date.Year, t.Date.Month, t.Type })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        return data
            .GroupBy(d => new { d.Year, d.Month })
            .Select(g => (
                g.Key.Year,
                g.Key.Month,
                Income:  g.Where(x => x.Type.ToLower() == "income").Sum(x => x.Total),
                Expense: g.Where(x => x.Type.ToLower() == "expense").Sum(x => x.Total)
            ))
            .OrderBy(x => x.Year).ThenBy(x => x.Month);
    }
}
