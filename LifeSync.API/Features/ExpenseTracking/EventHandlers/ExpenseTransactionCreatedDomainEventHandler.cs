using Microsoft.EntityFrameworkCore;
using LifeSync.API.Infrastructure.DomainEvents;
using LifeSync.API.Models.Events;
using LifeSync.API.Persistence;

namespace LifeSync.API.Features.ExpenseTracking.EventHandlers;

public class ExpenseTransactionCreatedDomainEventHandler : IDomainEventHandler<ExpenseTransactionCreatedDomainEvent>
{
    private readonly ApplicationDbContext databaseContext;

    public ExpenseTransactionCreatedDomainEventHandler(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }
    
    public async Task Handle(ExpenseTransactionCreatedDomainEvent domainEvent)
    {
        var userId = domainEvent.userId;

        var transaction = domainEvent.expenseTransaction;

        var userAccount = await databaseContext.Users.FirstOrDefaultAsync(x => x.Id == userId.ToString());

        if (userAccount is not null)
        {
            userAccount.Balance -= transaction.Amount;
        }
        
        await databaseContext.SaveChangesAsync();
    }
}