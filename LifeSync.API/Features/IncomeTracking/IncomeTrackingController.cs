using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Features.IncomeTracking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await incomeTrackingService.GetUserIncomesAsync(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPost(Name = nameof(AddIncome))]
    public async Task<IActionResult> AddIncome(AddIncomeDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await incomeTrackingService.AddIncomeAsync(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
