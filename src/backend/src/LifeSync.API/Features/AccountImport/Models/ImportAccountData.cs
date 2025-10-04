using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.AccountImport.Models;

public record ImportAccountData
{
    public ImportAccountProfile ProfileData { get; init; } = default!;

    public List<ImportAccountExpenseTransaction> ExpenseTransactions { get; init; } = default!;

    public List<ImportAccountIncomeTransaction> IncomeTransactions { get; init; } = default!;
}

public record ImportAccountProfile
{
    public required decimal BalanceAmount { get; init; }

    public required string BalanceCurrency { get; init; }

    public required Guid LanguageId { get; init; }
}

public record ImportAccountExpenseTransaction
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Description { get; init; }

    public required ExpenseType ExpenseType { get; init; }

    public required DateTime Date { get; init; }
}

public record ImportAccountIncomeTransaction
{
    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Description { get; init; }

    public required DateTime Date { get; init; }
}