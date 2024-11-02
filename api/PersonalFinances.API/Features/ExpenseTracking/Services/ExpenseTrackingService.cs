using Microsoft.EntityFrameworkCore;
using PersonalFinances.API.Features.ExpenseTracking.Models;
using PersonalFinances.API.Features.IncomeTracking.Models;
using PersonalFinances.API.Models;
using PersonalFinances.API.Persistence;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.ExpenseTracking.Services;

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
        await databaseContext.SaveChangesAsync();

        return expenseTransaction.Id;
    }
}