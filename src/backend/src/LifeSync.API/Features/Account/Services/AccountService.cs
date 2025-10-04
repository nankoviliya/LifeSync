using LifeSync.API.Features.Account.Models;
using LifeSync.API.Features.Account.ResultMessages;
using LifeSync.API.Features.Account.Services.Contracts;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Account.Services;

public class AccountService : BaseService, IAccountService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        ApplicationDbContext databaseContext,
        ILogger<AccountService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetUserAccountDataDto>> GetUserAccountData(
        string userId,
        CancellationToken cancellationToken)
    {
        GetUserAccountDataDto? userData = await _databaseContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new GetUserAccountDataDto
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Language = u.Language,
                BalanceAmount = u.Balance.Amount,
                BalanceCurrency = u.Balance.Currency.Code
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userData is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);

            return Failure<GetUserAccountDataDto>(AccountResultMessages.UserNotFound);
        }

        return Success(userData);
    }

    public async Task<MessageResult> ModifyUserAccountData(
        string userId,
        ModifyUserAccountDataDto data,
        CancellationToken cancellationToken)
    {
        User? userToUpdate = await _databaseContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userToUpdate is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);

            return FailureMessage(AccountResultMessages.UserNotFound);
        }

        if (!Guid.TryParse(data.LanguageId, out Guid parsedLanguageId))
        {
            _logger.LogWarning("Unable to parse LanguageId: {LanguageId} for userId: {UserId}", data.LanguageId,
                userId);

            return FailureMessage(AccountResultMessages.UnableToParseLanguageId);
        }

        userToUpdate.UpdateFirstName(data.FirstName.ToRequiredString());
        userToUpdate.UpdateLastName(data.LastName.ToRequiredString());
        userToUpdate.UpdateLanguage(parsedLanguageId.ToRequiredStruct());

        try
        {
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for userId: {UserId}", userId);

            return FailureMessage("An error occurred while updating the user profile.");
        }

        return SuccessMessage(AccountResultMessages.UserProfileUpdated);
    }
}