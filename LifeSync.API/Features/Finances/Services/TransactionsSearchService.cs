using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Features.Finances.ResultMessages;
using LifeSync.API.Features.Finances.Services.Contracts;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Finances.Services
{
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

        public async Task<DataResult<GetUserFinancialTransactionsResponse>> GetUserFinancialTransactionsAsync(string userId, GetUserFinancialTransactionsRequest request)
        {
            var userIdIsParsed = Guid.TryParse(userId, out Guid userIdGuid);

            if (!userIdIsParsed)
            {
                _logger.LogWarning("Invalid user id was provided: {UserId}, unable to parse", userId);

                return Failure<GetUserFinancialTransactionsResponse>(TransactionsSearchResultMessages.InvalidUserId);
            }

            var response = new GetUserFinancialTransactionsResponse();

            if (request.TransactionTypes.Contains(TransactionType.Expense))
            {
                var getExpenseTransactionsQuery = GetExpenseTransactionsQuery(userIdGuid, request);

                var userExpenseTransactions = await getExpenseTransactionsQuery.ToListAsync();

                response.Transactions.AddRange(userExpenseTransactions.Select(MapExpenseTransaction));

                response.ExpenseSummary = CalculateExpenseSummary(userExpenseTransactions);
            }

            if (request.TransactionTypes.Contains(TransactionType.Income))
            {
                var getIncomeTransactionsQuery = GetIncomeTransactionsQuery(userIdGuid, request);

                var userIncomeTransactions = await getIncomeTransactionsQuery.ToListAsync();

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

            decimal totalSpentOnNeeds = expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Needs).Sum(x => x.Amount.Amount);

            decimal totalSpentOnWants = expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Wants).Sum(x => x.Amount.Amount);

            decimal totalSpentOnSavings = expenseTransactions.Where(x => x.ExpenseType == ExpenseType.Savings).Sum(x => x.Amount.Amount);

            var summary = new ExpenseSummaryDto
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

            var summary = new IncomeSummaryDto
            {
                TotalIncome = totalIncome,
                Currency = incomeTransactions.FirstOrDefault()?.Amount.Currency.Code ?? string.Empty
            };

            return summary;
        }

        private IQueryable<ExpenseTransaction> GetExpenseTransactionsQuery(Guid userId, GetUserFinancialTransactionsRequest request)
        {
            var query = _databaseContext.ExpenseTransactions.AsQueryable();

            query = query.Where(x => x.UserId == userId);

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

        private IQueryable<IncomeTransaction> GetIncomeTransactionsQuery(Guid userId, GetUserFinancialTransactionsRequest request)
        {
            var query = _databaseContext.IncomeTransactions.AsQueryable();

            query = query.Where(x => x.UserId == userId);

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

        private GetExpenseFinancialTransactionDto MapExpenseTransaction(ExpenseTransaction x) =>
            new GetExpenseFinancialTransactionDto
            {
                Id = x.Id.ToString(),
                Amount = x.Amount.Amount,
                Currency = x.Amount.Currency.Code,
                Date = x.Date.ToString("yyyy-MM-dd"),
                Description = x.Description,
                ExpenseType = x.ExpenseType,
                TransactionType = TransactionType.Expense
            };

        private GetIncomeFinancialTransactionDto MapIncomeTransaction(IncomeTransaction x) =>
            new GetIncomeFinancialTransactionDto
            {
                Id = x.Id.ToString(),
                Amount = x.Amount.Amount,
                Currency = x.Amount.Currency.Code,
                Date = x.Date.ToString("yyyy-MM-dd"),
                Description = x.Description,
                TransactionType = TransactionType.Income
            };
    }
}
