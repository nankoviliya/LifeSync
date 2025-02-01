namespace LifeSync.API.Features.Translations.Services
{
    public interface ITranslationsService
    {
        Task<IReadOnlyDictionary<string, string>> GetTranslationsByLanguageCodeAsync(string languageCode);
    }
}
