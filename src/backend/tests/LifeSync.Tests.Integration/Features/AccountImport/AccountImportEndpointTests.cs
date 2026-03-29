using FluentAssertions;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LifeSync.Tests.Integration.Features.AccountImport;

public class AccountImportEndpointTests : IntegrationTestsBase
{
    public AccountImportEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Import_ShouldImportUserData_WhenTypeIsJsonAndValidJsonFileIsProvided()
    {
        // Arrange
        RegisterRequest registerRequest = DefaultUserAccount.RegisterUserRequest;
        LoginResponse loginResponse = await LoginUserAsync(registerRequest.Email, registerRequest.Password);
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        ImportAccountData importData = new()
        {
            ProfileData =
                new ImportAccountProfile { BalanceAmount = 1600.0m, BalanceCurrency = "BGN", LanguageCode = "en" },
            ExpenseTransactions = new List<ImportAccountExpenseTransaction>
            {
                new()
                {
                    Amount = 450.0m,
                    Currency = "BGN",
                    Description = "Groceries - Store Name#1",
                    ExpenseType = ExpenseType.Needs,
                    Date = new DateTime(2025, 11, 23, 19, 57, 18, DateTimeKind.Utc)
                }
            },
            IncomeTransactions = new List<ImportAccountIncomeTransaction>
            {
                new()
                {
                    Amount = 700.0m,
                    Currency = "BGN",
                    Description = "Salary - October 2025",
                    Date = new DateTime(2025, 11, 23, 19, 57, 18, DateTimeKind.Utc)
                }
            }
        };

        string jsonContent = JsonSerializer.Serialize(importData);
        using MultipartFormDataContent form = AccountImportDataHelper.CreateImportForm(
            AccountImportFileFormat.Json, jsonContent, "import.json", "application/json");

        // Act
        HttpResponseMessage importResponse = await HttpClient.PostAsync("/api/accountImport", form);

        // Assert - response
        importResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        string responseString = await importResponse.Content.ReadAsStringAsync();
        responseString.Should().Contain("Account data imported successfully.");

        // Assert - DB state
        User? updatedUser = await DbContext.Users
            .AsNoTracking()
            .Include(x => x.Language)
            .Include(x => x.ExpenseTransactions)
            .Include(x => x.IncomeTransactions)
            .FirstOrDefaultAsync(x => x.Email == registerRequest.Email);

        updatedUser.Should().NotBeNull();

        // Profile
        updatedUser!.Balance.Amount.Should().Be(importData.ProfileData.BalanceAmount);
        updatedUser.Balance.CurrencyCode.Should().Be(importData.ProfileData.BalanceCurrency);
        updatedUser.Language.Code.Should().BeEquivalentTo(importData.ProfileData.LanguageCode); // case-insensitive

        // Expense transactions
        updatedUser.ExpenseTransactions.Should().HaveCount(1);
        ExpenseTransaction savedExpense = updatedUser.ExpenseTransactions.Single();
        ImportAccountExpenseTransaction expectedExpense = importData.ExpenseTransactions.Single();
        savedExpense.Amount.Amount.Should().Be(expectedExpense.Amount);
        savedExpense.Amount.CurrencyCode.Should().Be(expectedExpense.Currency);
        savedExpense.Description.Should().Be(expectedExpense.Description);
        savedExpense.ExpenseType.Should().Be(expectedExpense.ExpenseType);
        savedExpense.Date.Should().Be(expectedExpense.Date);

        // Income transactions
        updatedUser.IncomeTransactions.Should().HaveCount(1);
        IncomeTransaction savedIncome = updatedUser.IncomeTransactions.Single();
        ImportAccountIncomeTransaction expectedIncome = importData.IncomeTransactions.Single();
        savedIncome.Amount.Amount.Should().Be(expectedIncome.Amount);
        savedIncome.Amount.CurrencyCode.Should().Be(expectedIncome.Currency);
        savedIncome.Description.Should().Be(expectedIncome.Description);
        savedIncome.Date.Should().Be(expectedIncome.Date);
    }
}