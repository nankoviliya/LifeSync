using LifeSync.API.Features.ExpenseTracking.Models;
using LifeSync.API.Features.ExpenseTracking.ResultMessages;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Expenses.Events;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.ExpenseTracking.Services;

public class ExpenseTrackingService : BaseService, IExpenseTrackingService
{
    private readonly ApplicationDbContext databaseContext;

    public ExpenseTrackingService(ApplicationDbContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    public async Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpensesAsync(string userId)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            return Failure<GetExpenseTransactionsResponse>(ExpenseTrackingResultMessages.InvalidUserId);
        }

        var userExpenseTransactions = await databaseContext.ExpenseTransactions
            .Where(x => x.UserId == userIdGuid)
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        var userExpenseTransactionsDto = userExpenseTransactions.Select(x => new GetExpenseDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            ExpenseType = x.ExpenseType,
            Description = x.Description
        }).ToList();

        var result = new GetExpenseTransactionsResponse
        {
            ExpenseTransactions = userExpenseTransactionsDto,
        };

        return Success(result);
    }

    public async Task<DataResult<Guid>> AddExpenseAsync(string userId, AddExpenseDto request)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            return Failure<Guid>(ExpenseTrackingResultMessages.InvalidUserId);
        }

        var expenseTransaction = new ExpenseTransaction
        {
            Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
            Date = request.Date,
            Description = request.Description,
            ExpenseType = request.ExpenseType,
            UserId = userIdGuid
        };

        await databaseContext.ExpenseTransactions.AddAsync(expenseTransaction);

        expenseTransaction.RaiseDomainEvent(new ExpenseTransactionCreatedDomainEvent(userIdGuid, expenseTransaction));

        await databaseContext.SaveChangesAsync();

        return Success(expenseTransaction.Id);
    }
}