using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Auth;
using FinanceDashboard.Business.Helpers;
using FinanceDashboard.Business.Services.Interfaces;
using FinanceDashboard.Data.Repositories.Interfaces;

namespace FinanceDashboard.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly JwtHelper _jwt;

    public AuthService(IUserRepository userRepo, JwtHelper jwt)
    {
        _userRepo = userRepo;
        _jwt = jwt;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email.ToLower());

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return ApiResponse<LoginResponseDto>.Fail("Invalid email or password");

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.Fail("Your account has been deactivated. Contact an administrator.");

        var accessToken  = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken       = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.LastLoginAt        = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            AccessToken  = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(60),
            User = new UserInfoDto
            {
                Id       = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email    = user.Email,
                Role     = user.Role?.Name ?? string.Empty
            }
        }, "Login successful");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);

        if (user == null)
            return ApiResponse<LoginResponseDto>.Fail("Invalid refresh token");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            return ApiResponse<LoginResponseDto>.Fail("Refresh token has expired. Please login again.");

        var newAccessToken  = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        user.RefreshToken       = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepo.UpdateAsync(user);

        return ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto
        {
            AccessToken  = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt    = DateTime.UtcNow.AddMinutes(60),
            User = new UserInfoDto
            {
                Id       = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email    = user.Email,
                Role     = user.Role?.Name ?? string.Empty
            }
        }, "Token refreshed successfully");
    }

    public async Task<ApiResponse> LogoutAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse.Fail("User not found");

        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;
        await _userRepo.UpdateAsync(user);

        return ApiResponse.Ok("Logged out successfully");
    }
}
