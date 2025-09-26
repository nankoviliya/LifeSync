using LifeSync.API.Features.Authentication.Models;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity.Data;
using RegisterRequest = LifeSync.API.Features.Authentication.Models.RegisterRequest;

namespace LifeSync.API.Features.Authentication.Services;

public interface IAuthService
{
    Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request);

    Task<MessageResult> RegisterAsync(RegisterRequest request);
}