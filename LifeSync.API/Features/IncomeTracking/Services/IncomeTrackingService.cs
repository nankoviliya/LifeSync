using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Models;
using LifeSync.API.Models.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.IncomeTracking.Services;

public class IncomeTrackingService : IIncomeTrackingService
{
    private readonly ApplicationDbContext databaseContext;

    public IncomeTrackingService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IEnumerable<GetIncomeDto>> GetUserIncomesAsync(Guid userId)
    {
        var userIncomeTransactions = await databaseContext.IncomeTransactions
            .Where(x => x.UserId == userId)
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
        });

        return userIncomeTransactionsDto;
    }

    public async Task<Guid> AddIncomeAsync(Guid userId, AddIncomeDto request)
    {
        var incomeTransaction = new IncomeTransaction
        {
            Id = Guid.NewGuid(),
            Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
            Date = request.Date,
            Description = request.Description,
            UserId = userId
        };

        await databaseContext.IncomeTransactions.AddAsync(incomeTransaction);

        incomeTransaction.RaiseDomainEvent(new IncomeTransactionCreatedDomainEvent(userId, incomeTransaction));

        await databaseContext.SaveChangesAsync();

        return incomeTransaction.Id;
    }
}