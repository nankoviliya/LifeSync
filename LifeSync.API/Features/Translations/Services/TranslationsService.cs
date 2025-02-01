using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LifeSync.API.Features.Translations.Services
{
    public class TranslationsService : ITranslationsService
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

        public async Task<IReadOnlyDictionary<string, string>> GetTranslationsByLanguageCodeAsync(string languageCode)
        {
            var language = await databaseContext.Languages
                .AsNoTracking()
                .Where(l => l.Code.Equals(languageCode))
                .FirstOrDefaultAsync();

            if (language is null)
            {
                throw new Exception("Language not found.");
            }

            if (!translationsCache.TryGetValue(languageCode, out var translations))
            {
                var filePath = Path.Combine(translationsPath, $"{languageCode}.json");

                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);

                    translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                   ?? [];
                }
                else
                {
                    translations = [];
                }

                translationsCache.TryAdd(languageCode, translations);
            }

            return translations;
        }
    }
}
