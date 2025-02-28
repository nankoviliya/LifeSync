using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.ExpenseTracking.Models
{
    public record GetUserExpenseTransactionsRequest
    {
        public int? Year { get; init; }

        public int? Month { get; init; }

        public ExpenseType? ExpenseType { get; init; }
    }
}
