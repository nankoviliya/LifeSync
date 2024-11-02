using Microsoft.AspNetCore.Mvc;
using PersonalFinances.API.Features.ExpenseTracking.Models;
using PersonalFinances.API.Features.ExpenseTracking.Services;

namespace PersonalFinances.API.Features.ExpenseTracking;

[ApiController]
[Route("[controller]")]
public class ExpenseTrackingController : ControllerBase
{
    private readonly IExpenseTrackingService expenseTrackingService;

    public ExpenseTrackingController(IExpenseTrackingService expenseTrackingService)
    {
        this.expenseTrackingService = expenseTrackingService;
    }
    
    [HttpGet(Name = nameof(Transactions))]
    public async Task<IActionResult> Transactions()
    {
        var result = await expenseTrackingService.GetUserExpensesAsync(Guid.Empty);
        
        return Ok(result);  
    }

    [HttpPost(Name = nameof(AddExpense))]
    public async Task<IActionResult> AddExpense(AddExpenseDto request)
    {
        var result = await expenseTrackingService.AddExpenseAsync(Guid.Empty, request);
        
        return Ok(result);  
    }
}