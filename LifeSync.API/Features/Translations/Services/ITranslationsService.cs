using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Translations.Services
{
    public interface ITranslationsService
    {
        Task<DataResult<IReadOnlyDictionary<string, string>>> GetTranslationsByLanguageCodeAsync(string languageCode);
    }
}
