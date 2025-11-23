using FluentAssertions;
using LifeSync.API.Features.AccountImport;
using LifeSync.API.Features.AccountImport.Models;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Models.ApplicationUser;
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
        TokenResponse tokenResponse = await LoginUserAsync(registerRequest.Email, registerRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        ImportAccountData importData = new()
        {
            ProfileData =
                new ImportAccountProfile
                {
                    BalanceAmount = 1600.0m,
                    BalanceCurrency = "BGN",
                    LanguageId = Guid.Parse("A0B10001-1A2B-4C3D-9E10-000000000001")
                },
            ExpenseTransactions = new List<ImportAccountExpenseTransaction>
            {
                new()
                {
                    Amount = 450.0m,
                    Currency = "BGN",
                    Description = "Groceries - Store Name#1",
                    ExpenseType = 0,
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

        using MultipartFormDataContent
            form = AccountImportDataHelper.CreateImportForm(AccountImportFileFormat.Json, jsonContent, "import.json",
                "application/json");

        HttpResponseMessage importResponse = await HttpClient.PostAsync("/api/accountImport", form);

        // Assert
        importResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        string responseString = await importResponse.Content.ReadAsStringAsync();

        responseString.Should().NotBeNullOrEmpty();
        responseString.Should().Contain("Account data imported successfully.");

        User? updatedUser = await DbContext.Users
            .AsNoTracking()
            .Include(x => x.ExpenseTransactions)
            .Include(x => x.IncomeTransactions)
            .FirstOrDefaultAsync(x => x.Email == registerRequest.Email);

        updatedUser.Should().NotBeNull();

        // Profile assertions
        updatedUser.Balance.Amount.Should().Be(importData.ProfileData.BalanceAmount);
        updatedUser.Balance.CurrencyCode.Should().Be(importData.ProfileData.BalanceCurrency);
        updatedUser.LanguageId.Should().Be(importData.ProfileData.LanguageId);

        // Transaction count assertions
        updatedUser.ExpenseTransactions.Should().HaveCount(importData.ExpenseTransactions.Count);
        updatedUser.IncomeTransactions.Should().HaveCount(importData.IncomeTransactions.Count);
    }
}