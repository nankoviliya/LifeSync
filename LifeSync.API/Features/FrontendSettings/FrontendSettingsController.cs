using LifeSync.API.Features.FrontendSettings.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.FrontendSettings;

[Route("api/[controller]")]
[ApiController]
public class FrontendSettingsController : ControllerBase
{
    private readonly IFrontendSettingsService frontendSettingsService;

    public FrontendSettingsController(IFrontendSettingsService frontendSettingsService)
    {
        this.frontendSettingsService = frontendSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFrontendSettings()
    {
        var result = await frontendSettingsService.GetFrontendSettingsAsync();

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
