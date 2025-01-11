using Microsoft.EntityFrameworkCore;
using LifeSync.API.Features.ExpenseTracking.Models;
using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Models;
using LifeSync.API.Models.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;

namespace LifeSync.API.Features.ExpenseTracking.Services;

public class ExpenseTrackingService: IExpenseTrackingService
{
    private readonly ApplicationDbContext databaseContext;

    public ExpenseTrackingService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<IEnumerable<GetExpenseDto>> GetUserExpensesAsync(Guid userId)
    {
        var userExpenseTransactions = await databaseContext.ExpenseTransactions
            .Where(x => x.UserId == userId)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        var userExpenseTransactionsDto = userExpenseTransactions.Select(x => new GetExpenseDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date,
            ExpenseType = x.ExpenseType,
            Description = x.Description
        });
        
        return userExpenseTransactionsDto;
    }

    public async Task<Guid> AddExpenseAsync(Guid userId, AddExpenseDto request)
    {
        var expenseTransaction = new ExpenseTransaction
        {
            Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
            Date = request.Date,
            Description = request.Description,
            ExpenseType = request.ExpenseType,
            UserId = userId
        };
        
        await databaseContext.ExpenseTransactions.AddAsync(expenseTransaction);
        
        expenseTransaction.RaiseDomainEvent(new ExpenseTransactionCreatedDomainEvent(userId, expenseTransaction));
        
        await databaseContext.SaveChangesAsync();

        return expenseTransaction.Id;
    }
}