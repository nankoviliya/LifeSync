using LifeSync.API.Features.Configuration.Models;

namespace LifeSync.API.Features.Configuration.Services
{
    public interface IFrontendSettingsService
    {
        Task<FrontendSettings> GetFrontendSettingsAsync();
    }
}
