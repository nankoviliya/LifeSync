using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Shared.Results;
using Microsoft.AspNetCore.Identity.Data;

namespace LifeSync.API.Features.Authentication.Services;

public interface IAuthService
{
    Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request);
}