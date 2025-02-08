using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
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

    public async Task<MessageResult> RegisterAsync(Models.RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Balance = new Money(request.Balance, Currency.FromCode(request.Currency)),
            CurrencyPreference = Currency.FromCode(request.Currency),
            LanguageId = request.LanguageId,
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToArray();

            return FailureMessage(errors);
        }

        return SuccessMessage("Successfully registered");
    }
}