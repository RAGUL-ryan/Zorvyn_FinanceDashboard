namespace FinanceDashboard.Data.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;         // Admin, Analyst, Viewer
    public string Description { get; set; } = string.Empty;

    // Navigation
    public ICollection<User> Users { get; set; } = new List<User>();
}
