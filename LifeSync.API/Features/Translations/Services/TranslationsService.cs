using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LifeSync.API.Features.Translations.Services
{
    public class TranslationsService : BaseService, ITranslationsService
    {
        private readonly string translationsPath;
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> translationsCache;
        private readonly ApplicationDbContext databaseContext;

        public TranslationsService(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
            translationsPath = Path.Combine(Directory.GetCurrentDirectory(), "Translations");
            translationsCache = new ConcurrentDictionary<string, Dictionary<string, string>>();
        }

        public async Task<DataResult<IReadOnlyDictionary<string, string>>> GetTranslationsByLanguageCodeAsync(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                return Failure<IReadOnlyDictionary<string, string>>("Language code must be provided.");
            }

            var language = await databaseContext.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Code.ToLower().Equals(languageCode.ToLower()));

            if (language is null)
            {
                return Failure<IReadOnlyDictionary<string, string>>(TranslationsResultMessages.LanguageNotFound);
            }

            if (!translationsCache.TryGetValue(languageCode, out var translations))
            {
                var filePath = Path.Combine(translationsPath, $"{languageCode}.json");

                if (!File.Exists(filePath))
                {
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
                    return Failure<IReadOnlyDictionary<string, string>>($"{TranslationsResultMessages.ErrorReadingTranslations}: {ex.Message}");
                }

                translationsCache.TryAdd(languageCode, translations);
            }

            return Success((IReadOnlyDictionary<string, string>)translations);
        }
    }
}
