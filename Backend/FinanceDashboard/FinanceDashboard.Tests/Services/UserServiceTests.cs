using FinanceDashboard.Business.DTOs.Users;
using FinanceDashboard.Business.Services;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace FinanceDashboard.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _roleRepoMock = new Mock<IRoleRepository>();
        _service      = new UserService(_userRepoMock.Object, _roleRepoMock.Object);
    }

    private static Role AdminRole => new() { Id = 1, Name = "Admin", Description = "Full access" };
    private static Role ViewerRole => new() { Id = 3, Name = "Viewer", Description = "Read only" };

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateUser_ValidData_ReturnsSuccess()
    {
        var dto = new CreateUserDto
        {
            FirstName = "John",
            LastName  = "Doe",
            Email     = "john.doe@example.com",
            Password  = "SecurePass@1",
            RoleId    = 1
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        _roleRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(AdminRole);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _userRepoMock.Setup(r => r.GetByIdWithRoleAsync(It.IsAny<int>()))
                     .ReturnsAsync(new User { Id = 1, FirstName = "John", LastName = "Doe", Email = dto.Email, Role = AdminRole, RoleId = 1, IsActive = true });

        var result = await _service.CreateUserAsync(dto);

        result.Success.Should().BeTrue();
        result.Data!.Email.Should().Be(dto.Email);
        result.Data.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsFailure()
    {
        var dto = new CreateUserDto
        {
            FirstName = "Jane",
            LastName  = "Doe",
            Email     = "existing@example.com",
            Password  = "SecurePass@1",
            RoleId    = 1
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                     .ReturnsAsync(new User { Id = 99, Email = dto.Email });

        var result = await _service.CreateUserAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateUser_InvalidRole_ReturnsFailure()
    {
        var dto = new CreateUserDto
        {
            FirstName = "Jane",
            LastName  = "Smith",
            Email     = "jane@example.com",
            Password  = "SecurePass@1",
            RoleId    = 99
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync((User?)null);
        _roleRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Role?)null);

        var result = await _service.CreateUserAsync(dto);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid role");
    }

    // ── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetUserById_ExistingUser_ReturnsUser()
    {
        var user = new User { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@test.com", Role = ViewerRole, RoleId = 3, IsActive = true };
        _userRepoMock.Setup(r => r.GetByIdWithRoleAsync(1)).ReturnsAsync(user);

        var result = await _service.GetUserByIdAsync(1);

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(1);
        result.Data.FullName.Should().Be("Alice Smith");
    }

    [Fact]
    public async Task GetUserById_NonExisting_ReturnsFailure()
    {
        _userRepoMock.Setup(r => r.GetByIdWithRoleAsync(999)).ReturnsAsync((User?)null);

        var result = await _service.GetUserByIdAsync(999);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    // ── Status ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task SetUserStatus_ExistingUser_ChangesStatus()
    {
        var user = new User { Id = 2, IsActive = true, FirstName = "Bob", LastName = "K", Email = "bob@k.com", RoleId = 3 };
        _userRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.SetUserStatusAsync(2, false);

        result.Success.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteUser_ExistingUser_SoftDeletes()
    {
        var user = new User { Id = 3, IsDeleted = false, FirstName = "Del", LastName = "User", Email = "del@x.com", RoleId = 3 };
        _userRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteUserAsync(3);

        result.Success.Should().BeTrue();
        user.IsDeleted.Should().BeTrue();
        user.IsActive.Should().BeFalse();
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsers_ReturnsAllUsers()
    {
        var users = new List<User>
        {
            new() { Id = 1, FirstName = "A", LastName = "B", Email = "a@b.com", Role = AdminRole,  RoleId = 1, IsActive = true },
            new() { Id = 2, FirstName = "C", LastName = "D", Email = "c@d.com", Role = ViewerRole, RoleId = 3, IsActive = true }
        };

        _userRepoMock.Setup(r => r.GetAllWithRolesAsync()).ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        result.Success.Should().BeTrue();
        result.Data!.Should().HaveCount(2);
    }
}
