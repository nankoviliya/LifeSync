using LifeSync.API.Features.Configuration.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Configuration
{
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
            var frontendSettings = await frontendSettingsService.GetFrontendSettingsAsync();

            return Ok(frontendSettings);
        }
    }
}
