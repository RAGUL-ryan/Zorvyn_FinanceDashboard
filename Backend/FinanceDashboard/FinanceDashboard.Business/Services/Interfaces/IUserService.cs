using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Users;

namespace FinanceDashboard.Business.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<IEnumerable<UserResponseDto>>> GetAllUsersAsync();
    Task<ApiResponse<UserResponseDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserResponseDto>> CreateUserAsync(CreateUserDto dto);
    Task<ApiResponse<UserResponseDto>> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<ApiResponse> DeleteUserAsync(int id);
    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ApiResponse> SetUserStatusAsync(int id, bool isActive);
}
