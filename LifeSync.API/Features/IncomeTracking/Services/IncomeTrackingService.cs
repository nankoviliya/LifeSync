using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Features.IncomeTracking.ResultMessages;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Incomes.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.IncomeTracking.Services;

public class IncomeTrackingService : BaseService, IIncomeTrackingService
{
    private readonly ApplicationDbContext databaseContext;

    public IncomeTrackingService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<DataResult<GetIncomeTransactionsResponse>> GetUserIncomesAsync(string userId)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            return Failure<GetIncomeTransactionsResponse>(IncomeTrackingResultMessages.InvalidUserId);
        }

        var userIncomeTransactions = await databaseContext.IncomeTransactions
            .Where(x => x.UserId == userIdGuid)
            .OrderByDescending(x => x.Date)
            .AsNoTracking()
            .ToListAsync();

        var userIncomeTransactionsDto = userIncomeTransactions.Select(x => new GetIncomeDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            Description = x.Description
        }).ToList();

        var response = new GetIncomeTransactionsResponse
        {
            IncomeTransactions = userIncomeTransactionsDto
        };

        return Success(response);
    }

    public async Task<DataResult<Guid>> AddIncomeAsync(string userId, AddIncomeDto request)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            return Failure<Guid>(IncomeTrackingResultMessages.InvalidUserId);
        }

        var incomeTransaction = new IncomeTransaction
        {
            Id = Guid.NewGuid(),
            Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
            Date = request.Date,
            Description = request.Description,
            UserId = userIdGuid
        };

        await databaseContext.IncomeTransactions.AddAsync(incomeTransaction);

        incomeTransaction.RaiseDomainEvent(new IncomeTransactionCreatedDomainEvent(userIdGuid, incomeTransaction));

        await databaseContext.SaveChangesAsync();

        return Success(incomeTransaction.Id);
    }
}