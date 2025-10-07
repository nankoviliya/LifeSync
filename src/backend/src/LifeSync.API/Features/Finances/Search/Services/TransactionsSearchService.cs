using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.API.Features.Finances.Shared.Models;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Finances.Search.Services;

public class TransactionsSearchService : BaseService, ITransactionsSearchService
{
    private readonly ApplicationDbContext _databaseContext;
    private readonly ILogger<TransactionsSearchService> _logger;

    public TransactionsSearchService(
        ApplicationDbContext databaseContext,
        ILogger<TransactionsSearchService> logger)
    {
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task<DataResult<SearchTransactionsResponse>> SearchTransactionsAsync(
        RequiredString userId,
        SearchTransactionsRequest request,
        CancellationToken cancellationToken)
    {
        SearchTransactionsResponse response = new()
        {
            Transactions = new List<FinancialTransactionDto>(),
            ExpenseSummary = new ExpenseSummaryDto(),
            IncomeSummary = new IncomeSummaryDto(),
            TransactionsCount = 0
        };

        if (request.TransactionTypes.Contains(TransactionType.Expense))
        {
            IQueryable<ExpenseTransaction> getExpenseTransactionsQuery = GetExpenseTransactionsQuery(userId, request);

            List<ExpenseTransaction> userExpenseTransactions =
                await getExpenseTransactionsQuery.ToListAsync(cancellationToken);

            response.Transactions.AddRange(userExpenseTransactions.Select(MapExpenseTransaction));

            response.ExpenseSummary = CalculateExpenseSummary(userExpenseTransactions);
        }

        if (request.TransactionTypes.Contains(TransactionType.Income))
        {
            IQueryable<IncomeTransaction> getIncomeTransactionsQuery = GetIncomeTransactionsQuery(userId, request);

            List<IncomeTransaction> userIncomeTransactions =
                await getIncomeTransactionsQuery.ToListAsync(cancellationToken);

            response.Transactions.AddRange(userIncomeTransactions.Select(MapIncomeTransaction));

            response.IncomeSummary = CalculateIncomeSummary(userIncomeTransactions);
        }

        response.Transactions = response.Transactions.OrderByDescending(x => x.Date).ToList();
        response.TransactionsCount = response.Transactions.Count;

        return Success(response);
    }

    private static ExpenseSummaryDto CalculateExpenseSummary(List<ExpenseTransaction> expenseTransactions)
    {
        decimal totalSpent = expenseTransactions.Sum(x => x.Amount.Amount);

        decimal totalSpentOnNeeds =
            expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Needs).Sum(x => x.Amount.Amount);

        decimal totalSpentOnWants =
            expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Wants).Sum(x => x.Amount.Amount);

        decimal totalSpentOnSavings = expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Savings)
            .Sum(x => x.Amount.Amount);

        ExpenseSummaryDto summary = new()
        {
            TotalSpent = totalSpent,
            TotalSpentOnNeeds = totalSpentOnNeeds,
            TotalSpentOnWants = totalSpentOnWants,
            TotalSpentOnSavings = totalSpentOnSavings,
            Currency = expenseTransactions.FirstOrDefault()?.Amount.Currency.Code ?? string.Empty
        };

        return summary;
    }

    private static IncomeSummaryDto CalculateIncomeSummary(List<IncomeTransaction> incomeTransactions)
    {
        decimal totalIncome = incomeTransactions.Sum(x => x.Amount.Amount);

        IncomeSummaryDto summary = new()
        {
            TotalIncome = totalIncome,
            Currency = incomeTransactions.FirstOrDefault()?.Amount.Currency.Code ?? string.Empty
        };

        return summary;
    }

    private IQueryable<ExpenseTransaction> GetExpenseTransactionsQuery(RequiredString userId,
        SearchTransactionsRequest request)
    {
        IQueryable<ExpenseTransaction> query = _databaseContext.ExpenseTransactions.AsQueryable();

        query = query.Where(x => x.UserId.Equals(userId.Value));

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

        if (request.ExpenseTypes is not null && request.ExpenseTypes.Count > 0)
        {
            query = query.Where(x => request.ExpenseTypes.Contains(x.ExpenseType));
        }

        query = query.AsNoTracking().OrderByDescending(x => x.Date);

        return query;
    }

    private IQueryable<IncomeTransaction> GetIncomeTransactionsQuery(RequiredString userId,
        SearchTransactionsRequest request)
    {
        IQueryable<IncomeTransaction> query = _databaseContext.IncomeTransactions.AsQueryable();

        query = query.Where(x => x.UserId.Equals(userId.Value));

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

        query = query.AsNoTracking().OrderByDescending(x => x.Date);

        return query;
    }

    private ExpenseTransactionDto MapExpenseTransaction(ExpenseTransaction x) =>
        new()
        {
            Id = x.Id.ToString(),
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            Description = x.Description,
            ExpenseType = x.ExpenseType,
            TransactionType = TransactionType.Expense
        };

    private IncomeTransactionDto MapIncomeTransaction(IncomeTransaction x) =>
        new()
        {
            Id = x.Id.ToString(),
            Amount = x.Amount.Amount,
            Currency = x.Amount.Currency.Code,
            Date = x.Date.ToString("yyyy-MM-dd"),
            Description = x.Description,
            TransactionType = TransactionType.Income
        };
}
