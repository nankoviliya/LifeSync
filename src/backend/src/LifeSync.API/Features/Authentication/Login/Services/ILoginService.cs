using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Login.Services;

public interface ILoginService
{
    Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request);
}