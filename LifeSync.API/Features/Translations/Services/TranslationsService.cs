using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LifeSync.API.Features.Translations.Services;

public class TranslationsService : BaseService, ITranslationsService
{
    private readonly string _translationsPath;
    private readonly ConcurrentDictionary<string, Dictionary<string, string>> _translationsCache;
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<TranslationsService> _logger;

    public TranslationsService(
        ApplicationDbContext databaseContext,
        ILogger<TranslationsService> logger)
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
            var filePath = Path.Combine(_translationsPath, $"{languageCode}.json");

            if (!File.Exists(filePath))
            {
                _logger.LogError("File with translations not found, Language Code: {LanguageCode}", languageCode);

                return Failure<IReadOnlyDictionary<string, string>>(TranslationsResultMessages.TranslationsFileNotFound);
            }

            try
            {
                var json = await File.ReadAllTextAsync(filePath);

                translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                               ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, TranslationsResultMessages.ErrorReadingTranslations);

                return Failure<IReadOnlyDictionary<string, string>>($"{TranslationsResultMessages.ErrorReadingTranslations}: {ex.Message}");
            }

            _translationsCache.TryAdd(languageCode, translations);
        }

        return Success((IReadOnlyDictionary<string, string>)translations);
    }
}
