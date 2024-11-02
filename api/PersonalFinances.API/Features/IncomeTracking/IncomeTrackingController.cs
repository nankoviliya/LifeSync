using Microsoft.AspNetCore.Mvc;
using PersonalFinances.API.Features.IncomeTracking.Models;
using PersonalFinances.API.Features.IncomeTracking.Services;

namespace PersonalFinances.API.Features.IncomeTracking;

[ApiController]
[Route("[controller]")]
public class IncomeTrackingController : ControllerBase
{
    private readonly IIncomeTrackingService incomeTrackingService;

    public IncomeTrackingController(IIncomeTrackingService incomeTrackingService)
    {
        this.incomeTrackingService = incomeTrackingService;
    }
    
    [HttpGet(Name = nameof(Transactions))]
    public async Task<IActionResult> Transactions()
    {
        var result = await incomeTrackingService.GetUserIncomesAsync(Guid.Empty);
        
        return Ok(result);  
    }

    [HttpPost(Name = nameof(AddIncome))]
    public async Task<IActionResult> AddIncome(AddIncomeDto request)
    {
        var result = await incomeTrackingService.AddIncomeAsync(Guid.Empty, request);
        
        return Ok(result);  
    }
}
