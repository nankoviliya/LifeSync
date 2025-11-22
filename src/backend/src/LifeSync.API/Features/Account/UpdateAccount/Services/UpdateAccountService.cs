using LifeSync.API.Features.Account.Shared;
using LifeSync.API.Features.Account.UpdateAccount.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Account.UpdateAccount.Services;

public class UpdateAccountService : BaseService, IUpdateAccountService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<UpdateAccountService> _logger;

    public UpdateAccountService(
        ApplicationDbContext databaseContext,
        ILogger<UpdateAccountService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<MessageResult> ModifyUserAccountDataAsync(
        RequiredString userId,
        UpdateAccountRequest data,
        CancellationToken cancellationToken)
    {
        User? userToUpdate = await _databaseContext.Users
            .Where(u => u.Id == userId.Value)
            .FirstOrDefaultAsync(cancellationToken);

        if (userToUpdate is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId.Value);

            return FailureMessage(AccountResultMessages.UserNotFound);
        }

        if (!Guid.TryParse(data.LanguageId, out Guid parsedLanguageId))
        {
            _logger.LogWarning("Unable to parse LanguageId: {LanguageId} for userId: {UserId}", data.LanguageId,
                userId.Value);

            return FailureMessage(AccountResultMessages.UnableToParseLanguageId);
        }

        userToUpdate.UpdateFirstName(data.FirstName.ToRequiredString());
        userToUpdate.UpdateLastName(data.LastName.ToRequiredString());
        userToUpdate.UpdateLanguage(parsedLanguageId.ToRequiredStruct());

        try
        {
            _databaseContext.Update(userToUpdate);
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for userId: {UserId}", userId.Value);

            return FailureMessage("An error occurred while updating the user profile.");
        }

        return SuccessMessage(AccountResultMessages.UserProfileUpdated);
    }
}