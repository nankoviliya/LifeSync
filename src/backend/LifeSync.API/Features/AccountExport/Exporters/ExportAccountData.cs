using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.AccountExport.Exporters;

public record ExportAccountData
{
    public ExportAccountProfile ProfileData { get; init; } = default!;

    public List<ExportAccountExpenseTransaction> ExpenseTransactions { get; init; } = default!;

    public List<ExportAccountIncomeTransaction> IncomeTransactions { get; init; } = default!;
}

public record ExportAccountProfile
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

public record ExportAccountExpenseTransaction
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }

    public DateTime Date { get; init; }
}

public record ExportAccountIncomeTransaction
{
    public Guid Id { get; init; }

    public decimal Amount { get; init; } = default!;

    public string Currency { get; init; } = default!;

    public string Description { get; init; } = default!;

    public DateTime Date { get; init; }
}

