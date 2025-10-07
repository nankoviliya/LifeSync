using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Features.Finances.Shared.ResultMessages;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeSync.API.Features.Finances.Expenses.Services;

public class ExpenseService : BaseService, IExpenseService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(
        ApplicationDbContext databaseContext,
        ILogger<ExpenseService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<Guid>> AddExpenseAsync(
        RequiredString userId,
        AddExpenseRequest request,
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction dbTransaction =
            await _databaseContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            User? user = await _databaseContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId.Value, cancellationToken);

            if (user is null)
            {
                return Failure<Guid>(FinancesResultMessages.UserNotFound);
            }

            Money expenseAmount = new(request.Amount, Currency.FromCode(request.Currency));

            if (user.Balance.Currency != expenseAmount.Currency)
            {
                return Failure<Guid>(FinancesResultMessages.CurrencyMismatch);
            }

            ExpenseTransaction transactionData = ExpenseTransaction.From(
                expenseAmount.ToRequiredReference(),
                request.Date.ToRequiredStruct(),
                request.Description.ToRequiredString(),
                request.ExpenseType,
                userId);

            await _databaseContext.ExpenseTransactions.AddAsync(transactionData, cancellationToken);

            user.Withdraw(expenseAmount);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            return Success(transactionData.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Concurrency conflict when adding expense for user {UserId}", userId);
            return Failure<Guid>(FinancesResultMessages.ConcurrencyConflict);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to add expense for user {UserId}: {Error}", userId, ex.Message);
            return Failure<Guid>(FinancesResultMessages.RequestFailed);
        }
    }
}
