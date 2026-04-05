using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Auth;

namespace FinanceDashboard.Business.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse> LogoutAsync(int userId);
}
