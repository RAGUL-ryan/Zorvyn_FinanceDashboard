using FinanceDashboard.Data.Entities;

namespace FinanceDashboard.Data.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdWithRoleAsync(int id);
    Task<IEnumerable<User>> GetAllWithRolesAsync();
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
