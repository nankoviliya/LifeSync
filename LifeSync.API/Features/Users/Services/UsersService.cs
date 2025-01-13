using LifeSync.API.Features.Users.Models;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
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
            BalanceAmount = u.Balance.Amount,
            BalanceCurrency = u.Balance.Currency.Code,
        })
        .FirstOrDefaultAsync();

        return userData;
    }
}