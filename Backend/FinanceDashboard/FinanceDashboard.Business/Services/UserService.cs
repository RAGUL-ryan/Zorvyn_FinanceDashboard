using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Users;
using FinanceDashboard.Business.Services.Interfaces;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;

namespace FinanceDashboard.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;

    public UserService(IUserRepository userRepo, IRoleRepository roleRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }

    public async Task<ApiResponse<IEnumerable<UserResponseDto>>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllWithRolesAsync();
        return ApiResponse<IEnumerable<UserResponseDto>>.Ok(users.Select(MapToDto));
    }

    public async Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdWithRoleAsync(id);
        if (user == null)
            return ApiResponse<UserResponseDto>.Fail($"User with ID {id} not found");

        return ApiResponse<UserResponseDto>.Ok(MapToDto(user));
    }

    public async Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Check email uniqueness
        var existing = await _userRepo.GetByEmailAsync(dto.Email.ToLower());
        if (existing != null)
            return ApiResponse<UserResponseDto>.Fail("A user with this email already exists");

        // Validate role
        var role = await _roleRepo.GetByIdAsync(dto.RoleId);
        if (role == null)
            return ApiResponse<UserResponseDto>.Fail("Invalid role specified");

        var user = new User
        {
            FirstName    = dto.FirstName.Trim(),
            LastName     = dto.LastName.Trim(),
            Email        = dto.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId       = dto.RoleId,
            IsActive     = true
        };

        await _userRepo.AddAsync(user);

        // Reload with role
        var created = await _userRepo.GetByIdWithRoleAsync(user.Id);
        return ApiResponse<UserResponseDto>.Ok(MapToDto(created!), "User created successfully");
    }

    public async Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepo.GetByIdWithRoleAsync(id);
        if (user == null)
            return ApiResponse<UserResponseDto>.Fail($"User with ID {id} not found");

        if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.LastName))  user.LastName  = dto.LastName.Trim();
        if (dto.IsActive.HasValue)                     user.IsActive  = dto.IsActive.Value;

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailCheck = await _userRepo.GetByEmailAsync(dto.Email.ToLower());
            if (emailCheck != null && emailCheck.Id != id)
                return ApiResponse<UserResponseDto>.Fail("Email is already in use by another user");
            user.Email = dto.Email.ToLower().Trim();
        }

        if (dto.RoleId.HasValue)
        {
            var role = await _roleRepo.GetByIdAsync(dto.RoleId.Value);
            if (role == null)
                return ApiResponse<UserResponseDto>.Fail("Invalid role specified");
            user.RoleId = dto.RoleId.Value;
        }

        await _userRepo.UpdateAsync(user);
        var updated = await _userRepo.GetByIdWithRoleAsync(id);
        return ApiResponse<UserResponseDto>.Ok(MapToDto(updated!), "User updated successfully");
    }

    public async Task<ApiResponse> DeleteUserAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return ApiResponse.Fail($"User with ID {id} not found");

        // Soft delete
        user.IsDeleted  = true;
        user.DeletedAt  = DateTime.UtcNow;
        user.IsActive   = false;
        await _userRepo.UpdateAsync(user);

        return ApiResponse.Ok("User deleted successfully");
    }

    public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse.Fail("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ApiResponse.Fail("Current password is incorrect");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _userRepo.UpdateAsync(user);

        return ApiResponse.Ok("Password changed successfully");
    }

    public async Task<ApiResponse> SetUserStatusAsync(int id, bool isActive)
    {
        var user = await _userRepo.GetByIdAsync(id);
        if (user == null)
            return ApiResponse.Fail($"User with ID {id} not found");

        user.IsActive = isActive;
        await _userRepo.UpdateAsync(user);

        return ApiResponse.Ok($"User {(isActive ? "activated" : "deactivated")} successfully");
    }

    private static UserResponseDto MapToDto(User u) => new()
    {
        Id          = u.Id,
        FirstName   = u.FirstName,
        LastName    = u.LastName,
        Email       = u.Email,
        IsActive    = u.IsActive,
        Role        = u.Role?.Name ?? string.Empty,
        RoleId      = u.RoleId,
        CreatedAt   = u.CreatedAt,
        LastLoginAt = u.LastLoginAt
    };
}
