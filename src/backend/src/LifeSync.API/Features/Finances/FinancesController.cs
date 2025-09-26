using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.Common.Extensions;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LifeSync.API.Features.Finances;

[ApiController]
[Authorize]
[Route("api/finances")]
public class FinancesController : ControllerBase
{
    private readonly ITransactionsSearchService _transactionsSearchService;
    private readonly IExpenseTransactionsManagement _expenseTransactionsManagement;
    private readonly IIncomeTransactionsManagement _incomeTransactionsManagement;

    public FinancesController(
        ITransactionsSearchService transactionsSearchService,
        IExpenseTransactionsManagement expenseTransactionsManagement,
        IIncomeTransactionsManagement incomeTransactionsManagement)
    {
        _transactionsSearchService = transactionsSearchService;
        _expenseTransactionsManagement = expenseTransactionsManagement;
        _incomeTransactionsManagement = incomeTransactionsManagement;
    }

    [HttpGet("transactions", Name = nameof(GetTransactions))]
    [EndpointSummary("Retrieves financial transactions.")]
    [EndpointDescription(
        "Returns object that contains a list of financial transactions for the authenticated user, filtered by query parameters.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserFinancialTransactionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] GetUserFinancialTransactionsRequest request,
        CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<GetUserFinancialTransactionsResponse>? result =
            await _transactionsSearchService.GetUserFinancialTransactionsAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpGet("transactions/expense", Name = nameof(GetExpenseTransactions))]
    [EndpointSummary("Retrieves expense transactions")]
    [EndpointDescription(
        "Gets object that contains expense transactions for the authenticated user based on the provided filters.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetExpenseTransactionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseTransactions(
        [FromQuery] GetUserExpenseTransactionsRequest request,
        CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<GetExpenseTransactionsResponse>? result =
            await _expenseTransactionsManagement.GetUserExpenseTransactionsAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("transactions/expense", Name = nameof(AddExpense))]
    [EndpointSummary("Adds an expense transaction")]
    [EndpointDescription("Creates a new expense transaction for the authenticated user using the provided details.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddExpense(
        AddExpenseDto request,
        CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<Guid>? result =
            await _expenseTransactionsManagement.AddExpenseAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpGet("transactions/income", Name = nameof(GetIncomeTransactions))]
    [EndpointSummary("Retrieves income transactions")]
    [EndpointDescription("Gets an object that contains income transactions for the authenticated user.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetIncomeTransactionsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetIncomeTransactions(CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<GetIncomeTransactionsResponse>? result =
            await _incomeTransactionsManagement.GetUserIncomesAsync(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost("transactions/income", Name = nameof(AddIncome))]
    [EndpointSummary("Adds an income transaction")]
    [EndpointDescription("Creates a new income transaction for the authenticated user using the provided details.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddIncome(
        AddIncomeDto request,
        CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<Guid>? result =
            await _incomeTransactionsManagement.AddIncomeAsync(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}