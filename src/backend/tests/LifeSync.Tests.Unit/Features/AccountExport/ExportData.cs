using LifeSync.API.Features.AccountExport.Models;

namespace LifeSync.UnitTests.Features.AccountExport;

public static class ExportData
{
    public static ExportAccountData GetData() =>
        new()
        {
            ProfileData = new ExportAccountProfile { BalanceAmount = 1000.50m, BalanceCurrency = "USD" },
            ExpenseTransactions =
            [
                new ExportAccountExpenseTransaction
                {
                    Id = Guid.NewGuid(), Amount = 50.00m, Currency = "USD", Description = "Test Expense"
                }
            ],
            IncomeTransactions =
            [
                new ExportAccountIncomeTransaction
                {
                    Id = Guid.NewGuid(), Amount = 1000.00m, Currency = "USD", Description = "Test Income"
                }
            ]
        };
}