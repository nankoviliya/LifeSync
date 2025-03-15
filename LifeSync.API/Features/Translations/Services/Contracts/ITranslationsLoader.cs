namespace LifeSync.API.Features.Translations.Services.Contracts;

public interface ITranslationsLoader
{
    Task<Dictionary<string, string>> LoadTranslationsAsync(string languageCode);
}
