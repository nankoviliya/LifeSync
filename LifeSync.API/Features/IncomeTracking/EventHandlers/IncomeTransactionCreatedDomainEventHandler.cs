

using Microsoft.EntityFrameworkCore;
using LifeSync.API.Infrastructure.DomainEvents;
using LifeSync.API.Models.Events;
using LifeSync.API.Persistence;

namespace LifeSync.API.Features.IncomeTracking.EventHandlers;

public class IncomeTransactionCreatedDomainEventHandler : IDomainEventHandler<IncomeTransactionCreatedDomainEvent>
{
    private readonly ApplicationDbContext databaseContext;

    public IncomeTransactionCreatedDomainEventHandler(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }
    
    public async Task Handle(IncomeTransactionCreatedDomainEvent domainEvent)
    {
        var userId = domainEvent.userId;

        var transaction = domainEvent.incomeTransaction;

        var userAccount = await databaseContext.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());

        if (userAccount is not null)
        {
            userAccount.Balance += transaction.Amount;
        }
        
        await databaseContext.SaveChangesAsync();
    }
}