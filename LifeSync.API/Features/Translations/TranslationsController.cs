using LifeSync.API.Features.Translations.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Translations
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationsController : ControllerBase
    {
        private readonly ITranslationsService translationsService;

        public TranslationsController(ITranslationsService translationsService)
        {
            this.translationsService = translationsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTranslations([FromQuery] string languageCode)
        {
            var result = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result.Data);
        }
    }
}
