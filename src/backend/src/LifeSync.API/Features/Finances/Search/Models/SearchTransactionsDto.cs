using FastEndpoints;
using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Models.Expenses;
using System.Text.Json.Serialization;

namespace LifeSync.API.Features.Finances.Search.Models;

public record GetUserFinancialTransactionsRequest
{
    public string? Description { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    [FromQuery(BindingSources = Source.QueryParam)]
    public List<ExpenseType>? ExpenseTypes { get; init; } = [];

    [FromQuery(BindingSources = Source.QueryParam)]
    public List<TransactionType> TransactionTypes { get; init; } = [];
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