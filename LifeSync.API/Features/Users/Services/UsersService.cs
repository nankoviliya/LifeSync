using LifeSync.API.Features.Users.Models;
using LifeSync.API.Features.Users.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Users.Services;

public class UsersService : BaseService, IUsersService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<UsersService> _logger;

    public UsersService(
        ApplicationDbContext databaseContext,
        ILogger<UsersService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetUserProfileDataDto>> GetUserProfileData(
        string userId,
        CancellationToken cancellationToken)
    {
        var userData = await _databaseContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new GetUserProfileDataDto
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Language = u.Language,
                BalanceAmount = u.Balance.Amount,
                BalanceCurrency = u.Balance.Currency.Code,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userData is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);

            return Failure<GetUserProfileDataDto>(UsersResultMessages.UserNotFound);
        }

        return Success(userData);
    }

    public async Task<MessageResult> ModifyUserProfileData(
        string userId,
        ModifyUserProfileDataDto data,
        CancellationToken cancellationToken)
    {
        var userToUpdate = await _databaseContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userToUpdate is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId);

            return FailureMessage(UsersResultMessages.UserNotFound);
        }

        if (!Guid.TryParse(data.LanguageId, out Guid parsedLanguageId))
        {
            _logger.LogWarning("Unable to parse LanguageId: {LanguageId} for userId: {UserId}", data.LanguageId, userId);

            return FailureMessage(UsersResultMessages.UnableToParseLanguageId);
        }

        userToUpdate.FirstName = data.FirstName;
        userToUpdate.LastName = data.LastName;
        userToUpdate.LanguageId = parsedLanguageId;

        try
        {
            await _databaseContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for userId: {UserId}", userId);

            return FailureMessage("An error occurred while updating the user profile.");
        }

        return SuccessMessage(UsersResultMessages.UserProfileUpdated);
    }
}