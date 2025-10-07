using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.Finances.Expenses.Models;

public record AddExpenseRequest
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTime Date { get; init; }

    public required string Description { get; init; }

    public required ExpenseType ExpenseType { get; init; }
}

public record AddExpenseResponse
{
    public Guid TransactionId { get; init; }
}
