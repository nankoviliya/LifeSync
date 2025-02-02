using LifeSync.API.Models;

namespace LifeSync.API.Features.ExpenseTracking.Models;

public class GetExpenseDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }
}