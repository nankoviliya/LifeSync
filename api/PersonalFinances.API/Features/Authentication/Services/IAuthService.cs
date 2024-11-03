using Microsoft.AspNetCore.Identity.Data;
using PersonalFinances.API.Features.Authentication.Models;

namespace PersonalFinances.API.Features.Authentication.Services;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request);
}