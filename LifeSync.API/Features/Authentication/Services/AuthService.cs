using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace LifeSync.API.Features.Authentication.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;

    public AuthService(
        JwtTokenGenerator jwtTokenGenerator,
        UserManager<User> userManager)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
    }

    public async Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Failure<TokenResponse>("Invalid email or password.");
        }

        var token = await _jwtTokenGenerator.GenerateJwtTokenAsync(user);

        if (token is null)
        {
            return Failure<TokenResponse>("Failed to generate token.");
        }

        return Success(token);
    }
}