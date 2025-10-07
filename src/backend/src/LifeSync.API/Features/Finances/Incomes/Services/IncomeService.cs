using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Features.Finances.Shared.ResultMessages;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeSync.API.Features.Finances.Incomes.Services;

public class IncomeService : BaseService, IIncomeService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<IncomeService> _logger;

    public IncomeService(
        ApplicationDbContext databaseContext,
        ILogger<IncomeService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<Guid>> AddIncomeAsync(
        RequiredString userId,
        AddIncomeRequest request,
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

            Money incomeAmount = new(request.Amount, Currency.FromCode(request.Currency));

            if (user.Balance.Currency != incomeAmount.Currency)
            {
                return Failure<Guid>(FinancesResultMessages.CurrencyMismatch);
            }

            IncomeTransaction transactionData = IncomeTransaction.From(
                incomeAmount.ToRequiredReference(),
                request.Date.ToRequiredStruct(),
                request.Description.ToRequiredString(),
                userId);

            await _databaseContext.IncomeTransactions.AddAsync(transactionData, cancellationToken);

            user.Deposit(incomeAmount);

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            return Success(transactionData.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            _logger.LogWarning(ex, "Concurrency conflict when adding income for user {UserId}", userId);
            return Failure<Guid>(FinancesResultMessages.ConcurrencyConflict);
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to add income for user {UserId}: {Error}", userId, ex.Message);
            return Failure<Guid>(FinancesResultMessages.RequestFailed);
        }
    }
}
