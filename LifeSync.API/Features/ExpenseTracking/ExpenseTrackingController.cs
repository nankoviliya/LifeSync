using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LifeSync.API.Features.ExpenseTracking.Models;
using LifeSync.API.Features.ExpenseTracking.Services;

namespace LifeSync.API.Features.ExpenseTracking;

[ApiController]
[Authorize]
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