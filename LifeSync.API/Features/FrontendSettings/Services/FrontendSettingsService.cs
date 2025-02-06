using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.FrontendSettings.Services;

public class FrontendSettingsService : BaseService, IFrontendSettingsService
{
    private readonly ApplicationDbContext databaseContext;

    public FrontendSettingsService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<DataResult<FrontendSettingsResponse>> GetFrontendSettingsAsync()
    {
        var languageOptions = await GetLanguageOptionsAsync();

        var frontendSettings = new FrontendSettingsResponse
        {
            LanguageOptions = languageOptions
        };

        return Success(frontendSettings);
    }

    private async Task<List<LanguageOption>> GetLanguageOptionsAsync()
    {
        var languageOptions = await databaseContext.Languages
            .AsNoTracking()
            .Select(l => new LanguageOption
            {
                Id = l.Id,
                Name = l.Name
            })
            .ToListAsync();

        return languageOptions;
    }
}
