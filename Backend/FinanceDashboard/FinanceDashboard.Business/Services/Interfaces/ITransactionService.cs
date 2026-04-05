using FinanceDashboard.Business.DTOs;
using FinanceDashboard.Business.DTOs.Transactions;

namespace FinanceDashboard.Business.Services.Interfaces;

public interface ITransactionService
{
    Task<ApiResponse<PagedResponseDto<TransactionResponseDto>>> GetTransactionsAsync(TransactionFilterDto filter);
    Task<ApiResponse<TransactionResponseDto>> GetTransactionByIdAsync(int id);
    Task<ApiResponse<TransactionResponseDto>> CreateTransactionAsync(CreateTransactionDto dto, int createdByUserId);
    Task<ApiResponse<TransactionResponseDto>> UpdateTransactionAsync(int id, UpdateTransactionDto dto);
    Task<ApiResponse> DeleteTransactionAsync(int id);
}
