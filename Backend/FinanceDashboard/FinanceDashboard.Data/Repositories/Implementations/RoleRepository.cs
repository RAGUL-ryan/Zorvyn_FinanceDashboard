using FinanceDashboard.Data.Context;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Data.Repositories.Implementations;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(FinanceDbContext context) : base(context) { }

    public async Task<Role?> GetByNameAsync(string name)
        => await _dbSet.FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
}
