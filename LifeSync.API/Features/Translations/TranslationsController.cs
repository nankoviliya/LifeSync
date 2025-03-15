using LifeSync.API.Features.Translations.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Translations;

[Route("api/translations")]
[ApiController]
public class TranslationsController : ControllerBase
{
    private readonly ITranslationsLoader translationsService;

    public TranslationsController(ITranslationsLoader translationsService)
    {
        this.translationsService = translationsService;
    }

    [HttpGet]
    [EndpointSummary("Retrieves translations for specified language code")]
    [EndpointDescription("Retrieves translations inside dictionary based on the provided language code.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyDictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
