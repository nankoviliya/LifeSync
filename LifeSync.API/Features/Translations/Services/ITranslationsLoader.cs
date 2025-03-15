using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Translations.Services;

public interface ITranslationsLoader
{
    Task<DataResult<IReadOnlyDictionary<string, string>>> GetTranslationsByLanguageCodeAsync(string languageCode);
}
