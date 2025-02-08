using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.ExpenseTracking.Models;

public class AddExpenseDto
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTime Date { get; init; }

    public required string Description { get; init; }

    public required ExpenseType ExpenseType { get; init; }
}