using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using RegisterRequest = LifeSync.API.Features.Authentication.Models.RegisterRequest;

namespace LifeSync.API.Features.Authentication.Services;

public class AuthService : BaseService, IAuthService
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        JwtTokenGenerator jwtTokenGenerator,
        UserManager<User> userManager,
        ILogger<AuthService> logger)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<DataResult<TokenResponse>> LoginAsync(LoginRequest request)
    {
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Failure<TokenResponse>("Invalid email or password.");
        }

        TokenResponse? token = await _jwtTokenGenerator.GenerateJwtTokenAsync(user);

        if (token is null)
        {
            return Failure<TokenResponse>("Failed to generate token.");
        }

        return Success(token);
    }

    public async Task<MessageResult> RegisterAsync(RegisterRequest request)
    {
        Money userBalance = new(request.Balance, Currency.FromCode(request.Currency));

        User user = User.From(
            request.Email.ToRequiredString(),
            request.Email.ToRequiredString(),
            request.FirstName.ToRequiredString(),
            request.LastName.ToRequiredString(),
            userBalance.ToRequiredReference(),
            Currency.FromCode(request.Currency).ToRequiredReference(),
            request.LanguageId.ToRequiredStruct()
        );

        IdentityResult? createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            string? errorDescriptions = string.Join("; ", createResult.Errors.Select(e => e.Description));

            _logger.LogWarning("Registration failed. Errors: {Errors}", errorDescriptions);

            return FailureMessage(createResult.Errors.Select(e => e.Description).ToArray());
        }

        return SuccessMessage("Successfully registered");
    }
}