using LifeSync.API.Features.AccountExport.Models;
using LifeSync.API.Models.Expenses;

namespace LifeSync.Tests.Unit.Features.AccountExport;

public static class ExportData
{
    public static ExportAccountData GetData() =>
        new()
        {
            ProfileData = new ExportAccountProfile
            {
                UserId = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@doe.com",
                BalanceAmount = 1000.50m,
                BalanceCurrency = "USD",
                LanguageCode = "en"
            },
            ExpenseTransactions =
            [
                new ExportAccountExpenseTransaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 50.00m,
                    Currency = "USD",
                    Description = "Test Expense",
                    ExpenseType = ExpenseType.Needs,
                    Date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            ],
            IncomeTransactions =
            [
                new ExportAccountIncomeTransaction
                {
                    Id = Guid.NewGuid(),
                    Amount = 1000.00m,
                    Currency = "USD",
                    Description = "Test Income",
                    Date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            ]
        };
}