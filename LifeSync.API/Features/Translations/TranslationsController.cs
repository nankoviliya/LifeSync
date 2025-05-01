using LifeSync.API.Features.Translations.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Translations;

[Route("api/translations")]
[ApiController]
public class TranslationsController : ControllerBase
{
    private readonly ITranslationsService translationsService;

    public TranslationsController(ITranslationsService translationsService)
    {
        this.translationsService = translationsService;
    }

    [HttpGet]
    [EndpointSummary("Retrieves translations for specified language code")]
    [EndpointDescription("Retrieves translations inside dictionary based on the provided language code.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyDictionary<string, string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTranslations(
        [FromQuery] string languageCode,
        CancellationToken cancellationToken)
    {
        var result = await translationsService.GetTranslationsByLanguageCodeAsync(languageCode, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }
}
