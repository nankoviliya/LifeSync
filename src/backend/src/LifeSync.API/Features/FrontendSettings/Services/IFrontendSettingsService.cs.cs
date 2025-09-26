using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.FrontendSettings.Services;

public interface IFrontendSettingsService
{
    Task<DataResult<FrontendSettingsResponse>> GetFrontendSettingsAsync(CancellationToken cancellationToken);
}