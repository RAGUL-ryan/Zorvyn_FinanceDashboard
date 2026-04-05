using FinanceDashboard.Business.DTOs.Auth;
using FinanceDashboard.Business.Helpers;
using FinanceDashboard.Business.Services;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FinanceDashboard.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]            = "TestSuperSecretKeyThatIsAtLeast32CharsLong!",
                ["Jwt:Issuer"]         = "TestIssuer",
                ["Jwt:Audience"]       = "TestAudience",
                ["Jwt:ExpiryMinutes"]  = "60"
            })
            .Build();

        var jwtHelper = new JwtHelper(config);
        _service      = new AuthService(_userRepoMock.Object, jwtHelper);
    }

    private static User BuildActiveUser() => new()
    {
        Id           = 1,
        FirstName    = "Test",
        LastName     = "User",
        Email        = "test@example.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Correct@Pass1"),
        IsActive     = true,
        RoleId       = 1,
        Role         = new Role { Id = 1, Name = "Admin" }
    };

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        var user = BuildActiveUser();
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.LoginAsync(new LoginRequestDto
        {
            Email    = "test@example.com",
            Password = "Correct@Pass1"
        });

        result.Success.Should().BeTrue();
        result.Data!.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        result.Data.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsFailure()
    {
        var user = BuildActiveUser();
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequestDto
        {
            Email    = "test@example.com",
            Password = "WrongPassword!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsFailure()
    {
        _userRepoMock.Setup(r => r.GetByEmailAsync("nobody@example.com")).ReturnsAsync((User?)null);

        var result = await _service.LoginAsync(new LoginRequestDto
        {
            Email    = "nobody@example.com",
            Password = "AnyPassword1!"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }

    [Fact]
    public async Task Login_InactiveUser_ReturnsFailure()
    {
        var user = BuildActiveUser();
        user.IsActive = false;
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequestDto
        {
            Email    = "test@example.com",
            Password = "Correct@Pass1"
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("deactivated");
    }

    [Fact]
    public async Task Logout_ValidUser_ClearsRefreshToken()
    {
        var user = BuildActiveUser();
        user.RefreshToken       = "some-token";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _userRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.LogoutAsync(1);

        result.Success.Should().BeTrue();
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiry.Should().BeNull();
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_ReturnsFailure()
    {
        var user = BuildActiveUser();
        user.RefreshToken       = "expired-token";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1); // already expired

        _userRepoMock.Setup(r => r.GetByRefreshTokenAsync("expired-token")).ReturnsAsync(user);

        var result = await _service.RefreshTokenAsync("expired-token");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("expired");
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsFailure()
    {
        _userRepoMock.Setup(r => r.GetByRefreshTokenAsync("invalid")).ReturnsAsync((User?)null);

        var result = await _service.RefreshTokenAsync("invalid");

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }
}
