using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Shared.Services;
using System.Text.Json;

namespace LifeSync.API.Features.Translations.Services;

public class TranslationsFileLoader : BaseService, ITranslationsLoader
{
    private readonly string _translationsPath;
    private readonly ILogger<TranslationsFileLoader> _logger;

    public TranslationsFileLoader(
        ILogger<TranslationsFileLoader> logger)
    {
        _logger = logger;
        _translationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Translations");
    }

    public async Task<Dictionary<string, string>> LoadTranslationsAsync(string languageCode)
    {
        var translations = new Dictionary<string, string>();
        var languageDir = Path.Combine(_translationsPath, languageCode);

        if (!Directory.Exists(languageDir))
        {
            _logger.LogWarning("Translations directory not found for language code: {LanguageCode}", languageCode);
            return translations;
        }

        var files = Directory.GetFiles(languageDir, "*.json");

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);

                var fileTranslations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                       ?? new Dictionary<string, string>();

                foreach (var kvp in fileTranslations)
                {
                    translations[kvp.Key] = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading translations from file: {File}", file);
            }
        }

        return translations;
    }
}
