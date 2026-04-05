using FinanceDashboard.Data.Context;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Data.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(FinanceDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
        => await _dbSet.Include(u => u.Role)
                       .FirstOrDefaultAsync(u => u.Email == email.ToLower());

    public async Task<User?> GetByIdWithRoleAsync(int id)
        => await _dbSet.Include(u => u.Role)
                       .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        => await _dbSet.Include(u => u.Role)
                       .OrderBy(u => u.LastName)
                       .ToListAsync();

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _dbSet.Include(u => u.Role)
                       .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}
