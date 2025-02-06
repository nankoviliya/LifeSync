using LifeSync.API.Features.Users.Models;
using LifeSync.API.Features.Users.ResultMessages;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Users.Services;

public class UsersService : BaseService, IUsersService
{
    private readonly ApplicationDbContext databaseContext;

    public UsersService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<DataResult<GetUserProfileDataDto>> GetUserProfileData(string userId)
    {
        var userData = await databaseContext.Users
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
            .FirstOrDefaultAsync();

        if (userData is null)
        {
            return Failure<GetUserProfileDataDto>(UsersResultMessages.UserNotFound);
        }

        return Success(userData);
    }

    public async Task<MessageResult> ModifyUserProfileData(string userId, ModifyUserProfileDataDto data)
    {
        var userToUpdate = await databaseContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (userToUpdate is null)
        {
            return FailureMessage(UsersResultMessages.UserNotFound);
        }

        if (!Guid.TryParse(data.LanguageId, out Guid parsedLanguageId))
        {
            return FailureMessage(UsersResultMessages.UnableToParseLanguageId);
        }

        userToUpdate.FirstName = data.FirstName;
        userToUpdate.LastName = data.LastName;
        userToUpdate.LanguageId = parsedLanguageId;

        await databaseContext.SaveChangesAsync();

        return SuccessMessage(UsersResultMessages.UserProfileUpdated);
    }
}