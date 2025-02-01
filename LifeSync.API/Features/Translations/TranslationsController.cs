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
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return BadRequest("Language code is required.");
            }

            var translations = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode);

            return Ok(translations);
        }
    }
}
