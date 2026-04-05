using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Transactions;
using FinanceDashboard.Business.Services.Interfaces;
using FinanceDashboard.Data.Entities;
using FinanceDashboard.Data.Repositories.Interfaces;

namespace FinanceDashboard.Business.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;

    public TransactionService(ITransactionRepository repo) => _repo = repo;

    public async Task<ApiResponse<PagedResponseDto<TransactionResponseDto>>> GetTransactionsAsync(TransactionFilterDto filter)
    {
        if (filter.Page < 1)    filter.Page     = 1;
        if (filter.PageSize < 1 || filter.PageSize > 100) filter.PageSize = 10;

        var (items, total) = await _repo.GetPagedAsync(
            filter.Page, filter.PageSize,
            filter.Type, filter.Category,
            filter.From, filter.To,
            filter.Search);

        return ApiResponse<PagedResponseDto<TransactionResponseDto>>.Ok(new PagedResponseDto<TransactionResponseDto>
        {
            Items      = items.Select(MapToDto),
            TotalCount = total,
            Page       = filter.Page,
            PageSize   = filter.PageSize
        });
    }

    public async Task<ApiResponse<TransactionResponseDto>> GetTransactionByIdAsync(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null)
            return ApiResponse<TransactionResponseDto>.Fail($"Transaction with ID {id} not found");

        return ApiResponse<TransactionResponseDto>.Ok(MapToDto(t));
    }

    public async Task<ApiResponse<TransactionResponseDto>> CreateTransactionAsync(CreateTransactionDto dto, int createdByUserId)
    {
        var transaction = new Transaction
        {
            Amount          = dto.Amount,
            Type            = dto.Type.Trim(),
            Category        = dto.Category.Trim(),
            Date            = dto.Date.ToUniversalTime(),
            Description     = dto.Description.Trim(),
            Notes           = dto.Notes?.Trim(),
            CreatedByUserId = createdByUserId
        };

        await _repo.AddAsync(transaction);
        return ApiResponse<TransactionResponseDto>.Ok(MapToDto(transaction), "Transaction created successfully");
    }

    public async Task<ApiResponse<TransactionResponseDto>> UpdateTransactionAsync(int id, UpdateTransactionDto dto)
    {
        var transaction = await _repo.GetByIdAsync(id);
        if (transaction == null)
            return ApiResponse<TransactionResponseDto>.Fail($"Transaction with ID {id} not found");

        if (dto.Amount.HasValue)                       transaction.Amount      = dto.Amount.Value;
        if (!string.IsNullOrWhiteSpace(dto.Type))      transaction.Type        = dto.Type.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Category))  transaction.Category    = dto.Category.Trim();
        if (dto.Date.HasValue)                         transaction.Date        = dto.Date.Value.ToUniversalTime();
        if (!string.IsNullOrWhiteSpace(dto.Description)) transaction.Description = dto.Description.Trim();
        if (dto.Notes != null)                         transaction.Notes       = dto.Notes.Trim();

        await _repo.UpdateAsync(transaction);
        return ApiResponse<TransactionResponseDto>.Ok(MapToDto(transaction), "Transaction updated successfully");
    }

    public async Task<ApiResponse> DeleteTransactionAsync(int id)
    {
        var transaction = await _repo.GetByIdAsync(id);
        if (transaction == null)
            return ApiResponse.Fail($"Transaction with ID {id} not found");

        // Soft delete
        transaction.IsDeleted = true;
        transaction.DeletedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(transaction);

        return ApiResponse.Ok("Transaction deleted successfully");
    }

    private static TransactionResponseDto MapToDto(Transaction t) => new()
    {
        Id          = t.Id,
        Amount      = t.Amount,
        Type        = t.Type,
        Category    = t.Category,
        Date        = t.Date,
        Description = t.Description,
        Notes       = t.Notes,
        CreatedBy   = t.CreatedByUser != null ? $"{t.CreatedByUser.FirstName} {t.CreatedByUser.LastName}" : "Unknown",
        CreatedAt   = t.CreatedAt,
        UpdatedAt   = t.UpdatedAt
    };
}
