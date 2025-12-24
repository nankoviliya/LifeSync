using LifeSync.API.Features.Authentication.Refresh.Models;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Refresh.Services;

public interface IRefreshTokenService
{
    Task<DataResult<RefreshResponse>> RefreshTokenAsync(HttpRequest request, HttpResponse response);
}