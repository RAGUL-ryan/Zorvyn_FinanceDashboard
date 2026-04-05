using FinanceDashboard.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Data.Context;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Role ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Name).IsRequired().HasMaxLength(50);
            e.HasIndex(r => r.Name).IsUnique();
            e.Property(r => r.Description).HasMaxLength(255);
            e.HasQueryFilter(r => !r.IsDeleted);
        });

        // ── User ──────────────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            e.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(255);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.HasOne(u => u.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(u => u.RoleId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(u => !u.IsDeleted);
        });

        // ── Transaction ───────────────────────────────────────────────────
        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);
            e.Property(t => t.Type).IsRequired().HasMaxLength(20);
            e.Property(t => t.Category).IsRequired().HasMaxLength(100);
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.Notes).HasMaxLength(1000);
            e.HasOne(t => t.CreatedByUser)
             .WithMany(u => u.Transactions)
             .HasForeignKey(t => t.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(t => !t.IsDeleted);
        });

        // ── Seed Data ─────────────────────────────────────────────────────
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin",    Description = "Full system access",             CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = 2, Name = "Analyst",  Description = "View records and analytics",    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Role { Id = 3, Name = "Viewer",   Description = "View dashboard data only",      CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed admin user  (password = Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            Id            = 1,
            FirstName     = "System",
            LastName      = "Admin",
            Email         = "admin@finance.com",
            PasswordHash  = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            IsActive      = true,
            RoleId        = 1,
            CreatedAt     = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }

    // Auto-set UpdatedAt on save
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;

        return base.SaveChangesAsync(cancellationToken);
    }
}
