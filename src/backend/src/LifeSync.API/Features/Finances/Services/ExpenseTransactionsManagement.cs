using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.ResultMessages;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Persistence;
using LifeSync.API.Shared;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeSync.API.Features.Finances.Services;

public class ExpenseTransactionsManagement : BaseService, IExpenseTransactionsManagement
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<ExpenseTransactionsManagement> _logger;

    public ExpenseTransactionsManagement(
        ApplicationDbContext databaseContext,
        ILogger<ExpenseTransactionsManagement> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpenseTransactionsAsync(
        string userId,
        GetUserExpenseTransactionsRequest request,
        CancellationToken cancellationToken)
    {
        IQueryable<ExpenseTransaction> query = GetExpenseTransactionsQuery(userId, request);

        List<ExpenseTransaction> userExpenseTransactions = await query.ToListAsync(cancellationToken);

        GetExpenseTransactionsResponse response = CalculateGetExpenseTransactionsResponse(userExpenseTransactions);

        return Success(response);
    }

    private GetExpenseTransactionsResponse CalculateGetExpenseTransactionsResponse(
        List<ExpenseTransaction> userExpenseTransactions)
    {
        List<GetExpenseDto> userExpenseTransactionsDto = userExpenseTransactions.Select(x => new GetExpenseDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            ExpenseType = x.ExpenseType,
            Description = x.Description
        }).ToList();

        decimal totalSpent = userExpenseTransactions.Sum(x => x.Amount.Amount);

        decimal totalSpentOnNeeds = userExpenseTransactions
            .Where(x => x.ExpenseType == ExpenseType.Needs)
            .Sum(x => x.Amount.Amount);

        decimal totalSpentOnWants = userExpenseTransactions
            .Where(x => x.ExpenseType == ExpenseType.Wants)
            .Sum(x => x.Amount.Amount);

        decimal totalSpentOnSavings = userExpenseTransactions
            .Where(x => x.ExpenseType == ExpenseType.Savings)
            .Sum(x => x.Amount.Amount);

        GetExpenseTransactionsResponse response = new GetExpenseTransactionsResponse
        {
            ExpenseTransactions = userExpenseTransactionsDto,
            ExpenseSummary = new ExpenseSummaryDto
            {
                TotalSpent = totalSpent,
                TotalSpentOnNeeds = totalSpentOnNeeds,
                TotalSpentOnWants = totalSpentOnWants,
                TotalSpentOnSavings = totalSpentOnSavings,
                // TODO: Make currency convertable and common across the request
                Currency = userExpenseTransactions.FirstOrDefault()?.Amount.Currency.Code ?? string.Empty
            },
            TransactionsCount = userExpenseTransactions.Count
        };

        return response;
    }

    private IQueryable<ExpenseTransaction> GetExpenseTransactionsQuery(
        string userId,
        GetUserExpenseTransactionsRequest request)
    {
        IQueryable<ExpenseTransaction> query = _databaseContext.ExpenseTransactions.AsQueryable();

        query = query.Where(x => x.UserId.Equals(userId));

        if (!string.IsNullOrEmpty(request.Description))
        {
            query = query.Where(x => x.Description.Contains(request.Description));
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(x => x.Date >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(x => x.Date <= request.EndDate.Value);
        }

        if (request.ExpenseType.HasValue)
        {
            query = query.Where(x => x.ExpenseType == request.ExpenseType);
        }

        query = query.AsNoTracking().OrderByDescending(x => x.Date);

        return query;
    }

    public async Task<DataResult<Guid>> AddExpenseAsync(
        string userId,
        AddExpenseDto request,
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction dbTransaction =
            await _databaseContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            User? user = await _databaseContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId.ToString(), cancellationToken);

            if (user is null)
            {
                return Failure<Guid>(ExpenseTrackingResultMessages.UserNotFound);
            }

            Money expenseAmount = new Money(request.Amount, Currency.FromCode(request.Currency));

            if (user.Balance.Currency != expenseAmount.Currency)
            {
                return Failure<Guid>(ExpenseTrackingResultMessages.CurrencyMismatch);
            }

            ExpenseTransaction transactionData = new ExpenseTransaction
            {
                Amount = expenseAmount,
                Date = request.Date,
                Description = request.Description,
                ExpenseType = request.ExpenseType,
                UserId = userId.ToString()
            };

            await _databaseContext.ExpenseTransactions.AddAsync(transactionData, cancellationToken);
            user.Balance -= expenseAmount;

            await _databaseContext.SaveChangesAsync(cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);

            return Success(transactionData.Id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict when adding expense for user {UserId}", userId);
            return Failure<Guid>(ExpenseTrackingResultMessages.ConcurrencyConflict);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add expense for user {UserId}: {Error}", userId, ex.Message);
            return Failure<Guid>(ExpenseTrackingResultMessages.RequestFailed);
        }
    }
}