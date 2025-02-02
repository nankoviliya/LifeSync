namespace LifeSync.API.Features.IncomeTracking.Models;

public class GetIncomeDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;
}