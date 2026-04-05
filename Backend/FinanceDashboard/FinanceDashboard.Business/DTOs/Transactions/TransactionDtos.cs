namespace FinanceDashboard.Business.DTOs.Transactions;

public class CreateTransactionDto
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;        // Income / Expense
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateTransactionDto
{
    public decimal? Amount { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
}

public class TransactionFilterDto
{
    public string? Type { get; set; }
    public string? Category { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class TransactionResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PagedResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
