using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.ExpenseTracking.Models;

public class GetExpenseTransactionsResponse
{
    public List<GetExpenseDto> ExpenseTransactions { get; init; } = [];

    public ExpenseSummaryDto ExpenseSummary { get; init; } = default!;

    public int TransactionsCount { get; init; }
}

public class ExpenseSummaryDto
{
    public decimal TotalSpent { get; init; }

    public decimal TotalSpentOnNeeds { get; init; }

    public decimal TotalSpentOnWants { get; init; }

    public decimal TotalSpentOnSavings { get; init; }

    public string Currency { get; init; } = default!;
}

public class GetExpenseDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }
}