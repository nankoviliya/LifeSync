using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PersonalFinances.API.Features.ExpenseTracking.Models;
using PersonalFinances.API.Features.ExpenseTracking.Services;

namespace PersonalFinances.API.Features.ExpenseTracking;

[ApiController]
[Route("api/expense")]
public class ExpenseTrackingController : ControllerBase
{
    private readonly IExpenseTrackingService expenseTrackingService;

    public ExpenseTrackingController(IExpenseTrackingService expenseTrackingService)
    {
        this.expenseTrackingService = expenseTrackingService;
    }
    
    [HttpGet("transactions", Name = nameof(GetExpenseTransactions))]
    public async Task<IActionResult> GetExpenseTransactions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var result = await expenseTrackingService.GetUserExpensesAsync(Guid.Parse(userId));

        return Ok(result);  
    }

    [HttpPost(Name = nameof(AddExpense))]
    public async Task<IActionResult> AddExpense(AddExpenseDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var result = await expenseTrackingService.AddExpenseAsync(Guid.Parse(userId), request);
        
        return Ok(result);  
    }
}