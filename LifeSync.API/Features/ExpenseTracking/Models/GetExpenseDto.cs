using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.ExpenseTracking.Models;

public class GetExpenseTransactionsResponse
{
    public List<GetExpenseDto> ExpenseTransactions { get; init; } = [];
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