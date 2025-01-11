using Microsoft.AspNetCore.Identity.Data;
using LifeSync.API.Features.Authentication.Models;

namespace LifeSync.API.Features.Authentication.Services;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request);
}