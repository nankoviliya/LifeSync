using LifeSync.API.Models.Expenses;
using System.Text.Json.Serialization;

namespace LifeSync.API.Features.Finances.Models
{
    public record GetUserFinancialTransactionsRequest
    {
        public string? Description { get; init; }

        public DateTime? StartDate { get; init; }

        public DateTime? EndDate { get; init; }

        public ExpenseType? ExpenseType { get; init; }

        public IReadOnlyCollection<TransactionType> TransactionTypes { get; init; } = [TransactionType.Expense, TransactionType.Income];
    }

    public class GetUserFinancialTransactionsResponse
    {
        public List<GetFinancialTransactionDto> Transactions { get; set; } = [];

        public ExpenseSummaryDto ExpenseSummary { get; set; } = default!;

        public IncomeSummaryDto IncomeSummary { get; set; } = default!;

        public int TransactionsCount { get; set; }
    }

    [JsonDerivedType(typeof(GetExpenseFinancialTransactionDto), "Expense")]
    [JsonDerivedType(typeof(GetIncomeFinancialTransactionDto), "Income")]
    public abstract class GetFinancialTransactionDto
    {
        public string Id { get; init; } = default!;

        public decimal Amount { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string Date { get; init; } = default!;

        public string Description { get; init; } = default!;

        public TransactionType TransactionType { get; init; }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }

    public class GetIncomeFinancialTransactionDto : GetFinancialTransactionDto
    {

    }

    public class GetExpenseFinancialTransactionDto : GetFinancialTransactionDto
    {
        public ExpenseType ExpenseType { get; init; }
    }
}
