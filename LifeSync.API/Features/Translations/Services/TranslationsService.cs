using LifeSync.API.Features.Translations.ResultMessages;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace LifeSync.API.Features.Translations.Services
{
    public class TranslationsService : BaseService, ITranslationsService
    {
        private readonly ConcurrentDictionary<string, Dictionary<string, string>> _translationsCache;
        private readonly ApplicationDbContext _databaseContext;
        private readonly ITranslationsLoader _translationsLoader;
        private readonly ILogger<TranslationsService> _logger;

        public TranslationsService(
            ApplicationDbContext databaseContext,
            ITranslationsLoader translationsLoader,
            ILogger<TranslationsService> logger)
        {
            _databaseContext = databaseContext;
            _translationsLoader = translationsLoader;
            _logger = logger;
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
                translations = await _translationsLoader.LoadTranslationsAsync(languageCode);
                _translationsCache.TryAdd(languageCode, translations);
            }

            return Success((IReadOnlyDictionary<string, string>)translations);
        }
    }
}
