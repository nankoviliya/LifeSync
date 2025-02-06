namespace LifeSync.API.Features.IncomeTracking.Models;

public class GetIncomeTransactionsResponse
{
    public List<GetIncomeDto> IncomeTransactions { get; init; } = new();
}

public class GetIncomeDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Date { get; init; } = default!;

    public string Description { get; init; } = default!;
}