using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LifeSync.API.Features.Translations.Services;

public class TranslationsFileLoader : BaseService, ITranslationsLoader
{
    private readonly string _translationsPath;
    private readonly ConcurrentDictionary<string, Dictionary<string, string>> _translationsCache;
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<TranslationsFileLoader> _logger;

    public TranslationsFileLoader(
        ApplicationDbContext databaseContext,
        ILogger<TranslationsFileLoader> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
        _translationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Translations");
        _translationsCache = new ConcurrentDictionary<string, Dictionary<string, string>>();
    }

    public async Task<DataResult<IReadOnlyDictionary<string, string>>> GetTranslationsByLanguageCodeAsync(string languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            _logger.LogWarning(TranslationsResultMessages.LanguageCodeNotProvided);

            return Failure<IReadOnlyDictionary<string, string>>(TranslationsResultMessages.LanguageCodeNotProvided);
        }

        var language = await _databaseContext.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Code.ToLower().Equals(languageCode.ToLower()));

        if (language is null)
        {
            _logger.LogError("Language not found, Language Code: {LanguageCode}", languageCode);

            return Failure<IReadOnlyDictionary<string, string>>(TranslationsResultMessages.LanguageNotFound);
        }

        if (!_translationsCache.TryGetValue(languageCode, out var translations))
        {
            translations = await LoadTranslationsAsync(languageCode);
            _translationsCache.TryAdd(languageCode, translations);
        }

        return Success((IReadOnlyDictionary<string, string>)translations);
    }

    private async Task<Dictionary<string, string>> LoadTranslationsAsync(string languageCode)
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
