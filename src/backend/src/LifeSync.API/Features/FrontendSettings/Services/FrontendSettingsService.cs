using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.FrontendSettings.Services;

public class FrontendSettingsService : BaseService, IFrontendSettingsService
{
    private readonly ApplicationDbContext _databaseContext;

    public FrontendSettingsService(ApplicationDbContext databaseContext) => _databaseContext = databaseContext;

    public async Task<DataResult<FrontendSettingsResponse>> GetFrontendSettingsAsync(
        CancellationToken cancellationToken)
    {
        List<LanguageOption>? languageOptions = await GetLanguageOptionsAsync(cancellationToken);

        List<CurrencyOption>? currencyOptions = GetCurrencyOptions();

        FrontendSettingsResponse? frontendSettings = new FrontendSettingsResponse
        {
            LanguageOptions = languageOptions, CurrencyOptions = currencyOptions
        };

        return Success(frontendSettings);
    }

    private async Task<List<LanguageOption>> GetLanguageOptionsAsync(
        CancellationToken cancellationToken)
    {
        List<LanguageOption>? languageOptions = await _databaseContext.Languages
            .AsNoTracking()
            .Select(l => new LanguageOption { Id = l.Id, Name = l.Name })
            .ToListAsync(cancellationToken);

        return languageOptions;
    }

    private static List<CurrencyOption> GetCurrencyOptions()
    {
        return CurrencyRegistry.All
            .Select(c => new CurrencyOption
            {
                Code = c.Code,
                Name = $"{c.Name} ({c.NativeName})"
            })
            .ToList();
    }
}