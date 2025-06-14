using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.API.Features.FrontendSettings.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.FrontendSettings;

[Route("api/frontendSettings")]
[ApiController]
public class FrontendSettingsController : ControllerBase
{
    private readonly IFrontendSettingsService frontendSettingsService;

    public FrontendSettingsController(IFrontendSettingsService frontendSettingsService)
    {
        this.frontendSettingsService = frontendSettingsService;
    }

    [HttpGet]
    [EndpointSummary("Retrieves frontend settings")]
    [EndpointDescription("Gets the current frontend settings configuration, like: language options, currency options etc.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FrontendSettingsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFrontendSettings(CancellationToken cancellationToken)
    {
        var result = await frontendSettingsService.GetFrontendSettingsAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
