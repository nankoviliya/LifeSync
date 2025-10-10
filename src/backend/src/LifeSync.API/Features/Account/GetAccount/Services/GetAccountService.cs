using LifeSync.API.Features.Account.GetAccount.Models;
using LifeSync.API.Features.Account.Shared;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Account.GetAccount.Services;

public class GetAccountService : BaseService, IGetAccountService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<GetAccountService> _logger;

    public GetAccountService(
        ApplicationDbContext databaseContext,
        ILogger<GetAccountService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetAccountResponse>> GetUserAccountDataAsync(
        RequiredString userId,
        CancellationToken cancellationToken)
    {
        GetAccountResponse? userData = await _databaseContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId.Value)
            .Select(u => new GetAccountResponse
            {
                UserId = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Language = u.Language,
                BalanceAmount = u.Balance.Amount,
                BalanceCurrency = u.Balance.CurrencyCode
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userData is null)
        {
            _logger.LogWarning("User not found for userId: {UserId}", userId.Value);

            return Failure<GetAccountResponse>(AccountResultMessages.UserNotFound);
        }

        return Success(userData);
    }
}
