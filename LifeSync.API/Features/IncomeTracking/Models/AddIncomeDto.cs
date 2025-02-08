namespace LifeSync.API.Features.IncomeTracking.Models;

public record AddIncomeDto
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required DateTime Date { get; init; }

    public required string Description { get; init; }
}