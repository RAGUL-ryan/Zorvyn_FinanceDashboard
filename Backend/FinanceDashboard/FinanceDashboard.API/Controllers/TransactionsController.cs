using FinanceDashboard.API.Filters;
using FinanceDashboard.Business.DTOs.Transactions;
using FinanceDashboard.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.API.Controllers;

/// <summary>
/// CRUD operations for financial transactions with filtering, pagination and search.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
        => _transactionService = transactionService;

    /// <summary>
    /// List transactions with optional filtering, search and pagination.
    /// [Admin, Analyst, Viewer]
    /// </summary>
    /// <param name="type">Filter by type: Income or Expense</param>
    /// <param name="category">Filter by category name</param>
    /// <param name="from">Start date (yyyy-MM-dd)</param>
    /// <param name="to">End date (yyyy-MM-dd)</param>
    /// <param name="search">Search in description, category, notes</param>
    /// <param name="page">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10, max 100)</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string?   type     = null,
        [FromQuery] string?   category = null,
        [FromQuery] DateTime? from     = null,
        [FromQuery] DateTime? to       = null,
        [FromQuery] string?   search   = null,
        [FromQuery] int       page     = 1,
        [FromQuery] int       pageSize = 10)
    {
        var filter = new TransactionFilterDto
        {
            Type     = type,
            Category = category,
            From     = from,
            To       = to,
            Search   = search,
            Page     = page,
            PageSize = pageSize
        };

        var result = await _transactionService.GetTransactionsAsync(filter);
        return Ok(result);
    }

    /// <summary>Get a single transaction by ID. [Admin, Analyst, Viewer]</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _transactionService.GetTransactionByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Create a new financial transaction. [Admin, Analyst]</summary>
    [HttpPost]
    [RequireRole("Admin", "Analyst")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        var userId = User.GetUserId();
        var result = await _transactionService.CreateTransactionAsync(dto, userId);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Update an existing transaction. [Admin, Analyst]</summary>
    [HttpPut("{id:int}")]
    [RequireRole("Admin", "Analyst")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto dto)
    {
        var result = await _transactionService.UpdateTransactionAsync(id, dto);
        if (!result.Success) return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Soft-delete a transaction. [Admin only]</summary>
    [HttpDelete("{id:int}")]
    [RequireRole("Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _transactionService.DeleteTransactionAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
