using LifeSync.API.Features.Configuration.Models;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Configuration.Services
{
    public class FrontendSettingsService : IFrontendSettingsService
    {
        private readonly ApplicationDbContext databaseContext;

        public FrontendSettingsService(ApplicationDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<FrontendSettings> GetFrontendSettingsAsync()
        {
            var languageOptions = await GetLanguageOptionsAsync();

            var frontendSettings = new FrontendSettings
            {
                LanguageOptions = languageOptions
            };

            return frontendSettings;
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
}
