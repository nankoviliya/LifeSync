namespace LifeSync.API.Features.Translations.Models;

public record GetTranslationsRequest
{
    public string LanguageCode { get; init; } = default!;
}
