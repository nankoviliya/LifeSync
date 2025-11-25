using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Models.Expenses;
using LifeSync.Common.Required;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text;

namespace LifeSync.Tests.Unit.Features.AccountImport;

public static class ImportData
{
    public static ImportAccountData GetData(RequiredStruct<Guid> languageId) =>
        new()
        {
            ProfileData =
                new ImportAccountProfile { BalanceAmount = 1000.50m, BalanceCurrency = "BGN", LanguageId = languageId },
            ExpenseTransactions = new List<ImportAccountExpenseTransaction>
            {
                new()
                {
                    Amount = 50.00m,
                    Currency = "BGN",
                    Description = "Test Expense",
                    ExpenseType = ExpenseType.Needs,
                    Date = new DateTime(2025, 1, 15)
                }
            },
            IncomeTransactions = new List<ImportAccountIncomeTransaction>
            {
                new()
                {
                    Amount = 2000.00m,
                    Currency = "BGN",
                    Description = "Test Income",
                    Date = new DateTime(2025, 1, 1)
                }
            }
        };

    public static IFormFile CreateSubstituteFormFile(string fileName, string content)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        MemoryStream stream = new(bytes);

        IFormFile? fileSubstitute = Substitute.For<IFormFile>();
        fileSubstitute.OpenReadStream().Returns(stream);
        fileSubstitute.FileName.Returns(fileName);
        fileSubstitute.Length.Returns(stream.Length);
        fileSubstitute.ContentType.Returns("application/json");

        return fileSubstitute;
    }
}