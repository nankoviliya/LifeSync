using LifeSync.API.Features.ExpenseTracking.Models;
using LifeSync.API.Features.ExpenseTracking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LifeSync.API.Features.ExpenseTracking;

[ApiController]
[Authorize]
[Route("api/expenses")]
public class ExpenseTrackingController : ControllerBase
{
    private readonly IExpenseTrackingService expenseTrackingService;

    public ExpenseTrackingController(IExpenseTrackingService expenseTrackingService)
    {
        this.expenseTrackingService = expenseTrackingService;
    }

    [HttpGet("transactions", Name = nameof(GetExpenseTransactions))]
    public async Task<IActionResult> GetExpenseTransactions([FromQuery] GetUserExpenseTransactionsRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await expenseTrackingService.GetUserExpenseTransactionsAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }

    [HttpPost(Name = nameof(AddExpense))]
    public async Task<IActionResult> AddExpense(AddExpenseDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await expenseTrackingService.AddExpenseAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result.Data);
    }
}