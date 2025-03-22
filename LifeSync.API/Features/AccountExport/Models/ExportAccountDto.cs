using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.AccountExport.Models;

public enum ExportAccountFileFormat
{
    Json = 1,
}

public record ExportAccountRequest
{
    public ExportAccountFileFormat Format { get; init; } = default!;
}

public record ExportAccountResult
{
    public byte[] Data { get; set; } = [];

    public string ContentType { get; set; } = default!;

    public string FileName { get; set; } = default!;
}

public record ExportAccountDto
{
    public ExportAccountProfileDto ProfileData { get; init; } = default!;

    public List<ExportAccountExpenseTransactionDto> ExpenseTransactions { get; init; } = default!;

    public List<ExportAccountIncomeTransactionDto> IncomeTransactions { get; init; } = default!;
}

public record ExportAccountProfileDto
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

public record ExportAccountExpenseTransactionDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }

    public DateTime Date { get; init; }
}

public record ExportAccountIncomeTransactionDto
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Description { get; init; } = default!;

    public DateTime Date { get; init; }
}
