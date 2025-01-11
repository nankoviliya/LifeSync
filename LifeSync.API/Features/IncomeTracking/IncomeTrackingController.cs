using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Features.IncomeTracking.Services;

namespace LifeSync.API.Features.IncomeTracking;

[ApiController]
[Authorize]
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var result = await incomeTrackingService.GetUserIncomesAsync(Guid.Parse(userId));
        
        return Ok(result);  
    }

    [HttpPost(Name = nameof(AddIncome))]
    public async Task<IActionResult> AddIncome(AddIncomeDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var result = await incomeTrackingService.AddIncomeAsync(Guid.Parse(userId), request);
        
        return Ok(result);  
    }
}
