using FinanceDashboard.Data.Entities;

namespace FinanceDashboard.Data.Repositories.Interfaces;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
}
