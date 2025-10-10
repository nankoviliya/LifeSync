using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Features.Authentication.Login.Services;

public class LoginService : BaseService, ILoginService
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;

    public LoginService(
        JwtTokenGenerator jwtTokenGenerator,
        UserManager<User> userManager)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
    }

    public async Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request)
    {
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Failure<TokenResponse>("Invalid email or password.");
        }

        TokenResponse token = await _jwtTokenGenerator.GenerateJwtTokenAsync(user);

        if (token is null)
        {
            return Failure<TokenResponse>("Failed to generate token.");
        }

        return Success(token);
    }
}