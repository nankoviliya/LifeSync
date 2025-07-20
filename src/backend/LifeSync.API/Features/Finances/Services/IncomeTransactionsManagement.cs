using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.ResultMessages;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Finances.Services;

public class IncomeTransactionsManagement : BaseService, IIncomeTransactionsManagement
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<IncomeTransactionsManagement> _logger;

    public IncomeTransactionsManagement(
        ApplicationDbContext databaseContext,
        ILogger<IncomeTransactionsManagement> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetIncomeTransactionsResponse>> GetUserIncomesAsync(
        string userId,
        CancellationToken cancellationToken)
    {
        var userIncomeTransactions = await _databaseContext.IncomeTransactions
            .Where(x => x.UserId.Equals(userId))
            .OrderByDescending(x => x.Date)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

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

    public async Task<DataResult<Guid>> AddIncomeAsync(
        string userId,
        AddIncomeDto request,
        CancellationToken cancellationToken)
    {
        await using var dbTransaction = await _databaseContext.Database.BeginTransactionAsync(cancellationToken);
   
        try
        {
            var user = await _databaseContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId.ToString(), cancellationToken);

            if (user is null)
                return Failure<Guid>(IncomeTrackingResultMessages.UserNotFound);

            var incomeAmount = new Money(request.Amount, Currency.FromCode(request.Currency));
       
            if (user.Balance.Currency != incomeAmount.Currency)
                return Failure<Guid>(IncomeTrackingResultMessages.CurrencyMismatch);
       
            var transactionData = new IncomeTransaction
            {
                Id = Guid.NewGuid(),
                Amount = new Money(request.Amount, Currency.FromCode(request.Currency)),
                Date = request.Date,
                Description = request.Description,
                UserId = userId.ToString()
            };
       
            await _databaseContext.IncomeTransactions.AddAsync(transactionData, cancellationToken);
            user.Balance += incomeAmount;
       
            await _databaseContext.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
       
            return Success(transactionData.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict when adding expense for user {UserId}", userId);
            return Failure<Guid>(IncomeTrackingResultMessages.ConcurrencyConflict);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add expense for user {UserId}: {Error}", userId, ex.Message);
            return Failure<Guid>(IncomeTrackingResultMessages.RequestFailed);
        }
    }
}
