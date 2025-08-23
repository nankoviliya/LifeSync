using LifeSync.API.Models.Expenses;

namespace LifeSync.API.Features.AccountImport.Importers;

public record ImportAccountData
{
    public ImportAccountProfile ProfileData { get; set; } = default!;

    public List<ImportAccountExpenseTransaction> ExpenseTransactions { get; set; } = default!;

    public List<ImportAccountIncomeTransaction> IncomeTransactions { get; set; } = default!;
}

public record ImportAccountProfile
{
    public required string UserId { get; init; }

    public string? UserName { get; init; }

    public string? Email { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public decimal? BalanceAmount { get; init; }

    public string? BalanceCurrency { get; init; }

    public Guid? LanguageId { get; init; }
}

public record ImportAccountExpenseTransaction
{
    public required Guid Id { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Description { get; init; }

    public required ExpenseType ExpenseType { get; init; }

    public required DateTime Date { get; init; }
}

public record ImportAccountIncomeTransaction
{
    public required Guid Id { get; init; }

    public required decimal Amount { get; init; }

    public required string Currency { get; init; }

    public required string Description { get; init; }

    public required DateTime Date { get; init; }
}
