using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Features.Authentication.Register.Services;

public class RegisterService : BaseService, IRegisterService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        UserManager<User> userManager,
        ILogger<RegisterService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<MessageResult> RegisterAsync(RegisterRequest request)
    {
        Money userBalance = new(request.Balance, request.Currency);

        User user = User.From(
            request.Email.ToRequiredString(),
            request.Email.ToRequiredString(),
            request.FirstName.ToRequiredString(),
            request.LastName.ToRequiredString(),
            userBalance.ToRequiredReference(),
            request.Currency.ToRequiredString(),
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