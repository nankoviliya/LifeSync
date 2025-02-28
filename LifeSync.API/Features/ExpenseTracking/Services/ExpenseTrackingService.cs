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
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<ExpenseTrackingService> _logger;

    public ExpenseTrackingService(
        ApplicationDbContext databaseContext,
        ILogger<ExpenseTrackingService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpenseTransactionsAsync(string userId, GetUserExpenseTransactionsRequest request)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            _logger.LogWarning("Invalid user id was provided: {UserId}, unable to parse", userId);

            return Failure<GetExpenseTransactionsResponse>(ExpenseTrackingResultMessages.InvalidUserId);
        }

        var query = GetExpenseTransactionsQuery(userIdGuid, request);

        var userExpenseTransactions = await query.ToListAsync();

        var response = CalculateGetExpenseTransactionsResponse(userExpenseTransactions);

        return Success(response);
    }

    private GetExpenseTransactionsResponse CalculateGetExpenseTransactionsResponse(List<ExpenseTransaction> userExpenseTransactions)
    {
        var userExpenseTransactionsDto = userExpenseTransactions.Select(x => new GetExpenseDto
        {
            Id = x.Id,
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            ExpenseType = x.ExpenseType,
            Description = x.Description
        }).ToList();

        decimal totalSpentThisMonth = userExpenseTransactions.Sum(x => x.Amount.Amount);

        decimal totalSpentThisMonthOnNeeds = userExpenseTransactions.Where(x => x.ExpenseType == ExpenseType.Needs).Sum(x => x.Amount.Amount);

        decimal totalSpentThisMonthOnWants = userExpenseTransactions.Where(x => x.ExpenseType == ExpenseType.Wants).Sum(x => x.Amount.Amount);

        decimal totalSpentThisMonthOnSavings = userExpenseTransactions.Where(x => x.ExpenseType == ExpenseType.Savings).Sum(x => x.Amount.Amount);

        var response = new GetExpenseTransactionsResponse
        {
            ExpenseTransactions = userExpenseTransactionsDto,
            ExpenseSummary = new ExpenseSummaryDto
            {
                TotalSpent = totalSpentThisMonth,
                TotalSpentOnNeeds = totalSpentThisMonthOnNeeds,
                TotalSpentOnWants = totalSpentThisMonthOnWants,
                TotalSpentOnSavings = totalSpentThisMonthOnSavings,
                // TODO: Make currency convertable and common across the request
                Currency = userExpenseTransactions.FirstOrDefault()?.Amount.Currency.Code ?? string.Empty
            },
            TransactionsCount = userExpenseTransactions.Count
        };

        return response;
    }

    private IQueryable<ExpenseTransaction> GetExpenseTransactionsQuery(Guid userId, GetUserExpenseTransactionsRequest request)
    {
        var query = _databaseContext.ExpenseTransactions.AsQueryable();

        query = query.Where(x => x.UserId == userId);

        if (request.Year.HasValue)
        {
            query = query.Where(x => x.Date.Year == request.Year.Value);
        }

        if (request.Month.HasValue)
        {
            query = query.Where(x => x.Date.Month == request.Month.Value);
        }

        if (request.ExpenseType.HasValue)
        {
            query = query.Where(x => x.ExpenseType == request.ExpenseType);
        }

        query = query.AsNoTracking().OrderByDescending(x => x.Date);

        return query;
    }

    public async Task<DataResult<Guid>> AddExpenseAsync(string userId, AddExpenseDto request)
    {
        var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

        if (!userIdIsParsed)
        {
            _logger.LogWarning("Invalid user id was provided: {UserId}, unable to parse", userId);

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

        await _databaseContext.ExpenseTransactions.AddAsync(expenseTransaction);

        expenseTransaction.RaiseDomainEvent(new ExpenseTransactionCreatedDomainEvent(userIdGuid, expenseTransaction));

        await _databaseContext.SaveChangesAsync();

        return Success(expenseTransaction.Id);
    }
}