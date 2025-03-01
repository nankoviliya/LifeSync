namespace LifeSync.API.Features.Finances.Models
{
    public record GetIncomeTransactionsResponse
    {
        public List<GetIncomeDto> IncomeTransactions { get; init; } = new();
    }

    public record GetIncomeDto
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string Date { get; init; } = default!;

        public string Description { get; init; } = default!;
    }

    public class IncomeSummaryDto
    {
        public decimal TotalIncome { get; init; }

        public string Currency { get; init; } = default!;
    }

    public record AddIncomeDto
    {
        public required decimal Amount { get; init; }

        public required string Currency { get; init; }

        public required DateTime Date { get; init; }

        public required string Description { get; init; }
    }
}
