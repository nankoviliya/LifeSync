using FastEndpoints;
using LifeSync.API.Features.Finances.Shared.Models;
using LifeSync.API.Models.Expenses;
using System.Text.Json.Serialization;

namespace LifeSync.API.Features.Finances.Search.Models;

public record SearchTransactionsRequest
{
    public string? Description { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    [FromQuery(BindingSources = Source.QueryParam)]
    public List<ExpenseType>? ExpenseTypes { get; init; } = [];

    [FromQuery(BindingSources = Source.QueryParam)]
    public List<TransactionType> TransactionTypes { get; init; } = [];
}

public class SearchTransactionsResponse
{
    public List<FinancialTransactionDto> Transactions { get; set; } = [];

    public ExpenseSummaryDto ExpenseSummary { get; set; } = default!;

    public IncomeSummaryDto IncomeSummary { get; set; } = default!;

    public int TransactionsCount { get; set; }
}

[JsonDerivedType(typeof(ExpenseTransactionDto), "Expense")]
[JsonDerivedType(typeof(IncomeTransactionDto), "Income")]
public abstract class FinancialTransactionDto
{
    public string Id { get; init; } = default!;

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;

    public TransactionType TransactionType { get; init; }
}

public class IncomeTransactionDto : FinancialTransactionDto
{
}

public class ExpenseTransactionDto : FinancialTransactionDto
{
    public ExpenseType ExpenseType { get; init; }
}

public class ExpenseSummaryDto
{
    public decimal TotalSpent { get; set; }

    public decimal TotalSpentOnNeeds { get; set; }

    public decimal TotalSpentOnWants { get; set; }

    public decimal TotalSpentOnSavings { get; set; }

    public string Currency { get; set; } = default!;
}

public class IncomeSummaryDto
{
    public decimal TotalIncome { get; set; }

    public string Currency { get; set; } = default!;
}
