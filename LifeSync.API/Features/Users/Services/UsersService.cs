using LifeSync.API.Features.Users.Models;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Users.Services;

public class UsersService : IUsersService
{
    private readonly ApplicationDbContext databaseContext;

    public UsersService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<GetUserProfileDataDto?> GetUserProfileData(string userId)
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

        return userData;
    }

    // TODO: enhance return type with implementation of a result object
    public async Task ModifyUserProfileData(string userId, ModifyUserProfileDataDto data)
    {
        var userToUpdate = await databaseContext.Users
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();

        if (userToUpdate is null)
        {
            return;
        }

        if (!Guid.TryParse(data.LanguageId, out Guid parsedLanguageId))
        {
            return;
        }

        userToUpdate.FirstName = data.FirstName;
        userToUpdate.LastName = data.LastName;
        userToUpdate.LanguageId = parsedLanguageId;

        await databaseContext.SaveChangesAsync();
    }
}