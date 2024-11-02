using Microsoft.AspNetCore.Mvc;
using PersonalFinances.API.Features.IncomeTracking.Models;
using PersonalFinances.API.Features.IncomeTracking.Services;

namespace PersonalFinances.API.Features.IncomeTracking;

[ApiController]
[Route("api/income")]
public class IncomeTrackingController : ControllerBase
{
    private readonly IIncomeTrackingService incomeTrackingService;

    public IncomeTrackingController(IIncomeTrackingService incomeTrackingService)
    {
        this.incomeTrackingService = incomeTrackingService;
    }
    
    [HttpGet("transactions", Name = nameof(GetIncomeTransactions))]
    public async Task<IActionResult> GetIncomeTransactions()
    {
        var result = await incomeTrackingService.GetUserIncomesAsync(Guid.NewGuid());
        
        return Ok(result);  
    }

    [HttpPost(Name = nameof(AddIncome))]
    public async Task<IActionResult> AddIncome(AddIncomeDto request)
    {
        var result = await incomeTrackingService.AddIncomeAsync(Guid.NewGuid(), request);
        
        return Ok(result);  
    }
}
