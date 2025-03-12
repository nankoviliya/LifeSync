using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.Finances.Models;

public record GetUserExpenseTransactionsRequest
{
    public string? Description { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? EndDate { get; init; }

    public ExpenseType? ExpenseType { get; init; }
}

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

public record GetExpenseDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }
}

public record AddExpenseDto
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTime Date { get; init; }

    public required string Description { get; init; }

    public required ExpenseType ExpenseType { get; init; }
}
