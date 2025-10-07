namespace LifeSync.API.Features.Finances.Incomes.Models;

public record AddIncomeRequest
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTime Date { get; init; }

    public required string Description { get; init; }
}

public record AddIncomeResponse
{
    public Guid TransactionId { get; init; }
}
