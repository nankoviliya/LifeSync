using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.AccountDataExchange.Models
{
    public enum ExportAccountFileFormat
    {
        Json = 1,
    }
    public record ExportAccountFileResultDto
    {
        public byte[] Data { get; set; } = [];

        public string ContentType { get; set; } = default!;

        public string FileName { get; set; } = default!;
    }

    public record ExportAccountDataDto
    {
        public ExportAccountProfileDataDto ProfileData { get; init; } = default!;

        public List<ExportAccountExpenseTransactionDataDto> ExpenseTransactions { get; init; } = default!;

        public List<ExportAccountIncomeTransactionDataDto> IncomeTransactions { get; init; } = default!;
    }

    public record ExportAccountProfileDataDto
    {
        public string UserId { get; init; } = default!;

        public string? UserName { get; init; }

        public string? Email { get; init; }

        public string FirstName { get; init; } = default!;

        public string LastName { get; init; } = default!;

        public decimal BalanceAmount { get; init; } = default!;

        public string BalanceCurrency { get; init; } = default!;

        public Guid LanguageId { get; init; }

        public string LanguageCode { get; init; } = default!;
    }

    public record ExportAccountExpenseTransactionDataDto
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string Description { get; init; } = default!;

        public ExpenseType ExpenseType { get; init; }
    }

    public record ExportAccountIncomeTransactionDataDto
    {
        public Guid Id { get; init; }

        public decimal Amount { get; init; } = default!;

        public string Currency { get; init; } = default!;

        public string Description { get; init; } = default!;
    }
}
