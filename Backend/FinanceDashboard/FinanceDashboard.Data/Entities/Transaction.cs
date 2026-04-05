namespace FinanceDashboard.Data.Entities;

public class Transaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;         // Income / Expense
    public string Category { get; set; } = string.Empty;     // e.g. Salary, Rent, Food
    public DateTime Date { get; set; }
    public string? Notes { get; set; }
    public string Description { get; set; } = string.Empty;

    // Foreign Key
    public int CreatedByUserId { get; set; }

    // Navigation
    public User CreatedByUser { get; set; } = null!;
}
