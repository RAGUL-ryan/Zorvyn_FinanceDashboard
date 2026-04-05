using FinanceDashboard.Business.DTOs.Transactions;
using FinanceDashboard.Business.Services;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;
using FluentAssertions;
using Moq;

namespace FinanceDashboard.Tests.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _repoMock;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _repoMock = new Mock<ITransactionRepository>();
        _service  = new TransactionService(_repoMock.Object);
    }

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateTransaction_ValidInput_ReturnsSuccess()
    {
        var dto = new CreateTransactionDto
        {
            Amount      = 1500.00m,
            Type        = "Income",
            Category    = "Salary",
            Date        = DateTime.UtcNow.AddDays(-1),
            Description = "Monthly salary"
        };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                 .ReturnsAsync((Transaction t) => t);

        var result = await _service.CreateTransactionAsync(dto, createdByUserId: 1);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Amount.Should().Be(1500.00m);
        result.Data.Type.Should().Be("Income");
        result.Data.Category.Should().Be("Salary");
    }

    [Fact]
    public async Task CreateTransaction_CallsRepositoryAddAsync()
    {
        var dto = new CreateTransactionDto
        {
            Amount      = 200m,
            Type        = "Expense",
            Category    = "Food",
            Date        = DateTime.UtcNow.AddDays(-2),
            Description = "Grocery shopping"
        };

        Transaction? captured = null;
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                 .Callback<Transaction>(t => captured = t)
                 .ReturnsAsync((Transaction t) => t);

        await _service.CreateTransactionAsync(dto, createdByUserId: 5);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        captured.Should().NotBeNull();
        captured!.CreatedByUserId.Should().Be(5);
    }

    // ── Get by ID ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetTransactionById_ExistingId_ReturnsTransaction()
    {
        var transaction = new Transaction
        {
            Id          = 1,
            Amount      = 500m,
            Type        = "Expense",
            Category    = "Utilities",
            Date        = DateTime.UtcNow,
            Description = "Electric bill"
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transaction);

        var result = await _service.GetTransactionByIdAsync(1);

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(1);
        result.Data.Amount.Should().Be(500m);
    }

    [Fact]
    public async Task GetTransactionById_NonExistingId_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Transaction?)null);

        var result = await _service.GetTransactionByIdAsync(99);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateTransaction_ExistingId_UpdatesFields()
    {
        var transaction = new Transaction
        {
            Id          = 1,
            Amount      = 100m,
            Type        = "Expense",
            Category    = "Food",
            Date        = DateTime.UtcNow,
            Description = "Old description"
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transaction);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        var dto = new UpdateTransactionDto { Amount = 250m, Description = "Updated description" };
        var result = await _service.UpdateTransactionAsync(1, dto);

        result.Success.Should().BeTrue();
        result.Data!.Amount.Should().Be(250m);
        result.Data.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateTransaction_NonExistingId_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Transaction?)null);

        var result = await _service.UpdateTransactionAsync(99, new UpdateTransactionDto());

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteTransaction_ExistingId_SoftDeletesRecord()
    {
        var transaction = new Transaction
        {
            Id          = 1,
            Amount      = 100m,
            Type        = "Income",
            Category    = "Other",
            Date        = DateTime.UtcNow,
            Description = "Test",
            IsDeleted   = false
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transaction);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        var result = await _service.DeleteTransactionAsync(1);

        result.Success.Should().BeTrue();
        transaction.IsDeleted.Should().BeTrue();
        transaction.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteTransaction_NonExistingId_ReturnsFailure()
    {
        _repoMock.Setup(r => r.GetByIdAsync(55)).ReturnsAsync((Transaction?)null);

        var result = await _service.DeleteTransactionAsync(55);

        result.Success.Should().BeFalse();
    }

    // ── GetPaged ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetTransactions_WithFilter_ReturnsPaged()
    {
        var transactions = new List<Transaction>
        {
            new() { Id = 1, Amount = 100m, Type = "Income",  Category = "Salary", Date = DateTime.UtcNow, Description = "A" },
            new() { Id = 2, Amount = 50m,  Type = "Expense", Category = "Food",   Date = DateTime.UtcNow, Description = "B" }
        };

        _repoMock.Setup(r => r.GetPagedAsync(1, 10, null, null, null, null, null))
                 .ReturnsAsync((transactions, 2));

        var result = await _service.GetTransactionsAsync(new TransactionFilterDto { Page = 1, PageSize = 10 });

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(2);
        result.Data.Items.Should().HaveCount(2);
    }
}
