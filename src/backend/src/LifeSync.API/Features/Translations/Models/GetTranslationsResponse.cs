namespace LifeSync.API.Features.Translations.Models;

public record GetTranslationsResponse
{
    public IReadOnlyDictionary<string, string> Translations { get; init; } = default!;
}
