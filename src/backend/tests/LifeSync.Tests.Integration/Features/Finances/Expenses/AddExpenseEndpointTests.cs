using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Expenses;
using LifeSync.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Finances.Expenses;

public class AddExpenseEndpointTests : IntegrationTestsBase
{
    public AddExpenseEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Add_AddsANewExpense_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        TokenResponse tokenResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        AddExpenseRequest request = new()
        {
            Amount = 500,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Groceries - Store Name",
            ExpenseType = ExpenseType.Needs
        };

        HttpResponseMessage addExpenseResponse =
            await HttpClient.PostAsJsonAsync($"/api/finances/transactions/expense", request);

        addExpenseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AddExpenseResponse? responseData = await addExpenseResponse.Content.ReadFromJsonAsync<AddExpenseResponse>();

        responseData.Should().NotBeNull();
        responseData.TransactionId.Should().NotBeEmpty();

        ExpenseTransaction? transaction =
            await DbContext.ExpenseTransactions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == responseData.TransactionId);

        transaction.Should().NotBeNull();
        transaction.Amount.Amount.Should().Be(request.Amount);
        transaction.Amount.CurrencyCode.Should().Be(request.Currency);
        transaction.Date.Should().Be(request.Date);
        transaction.Description.Should().Be(request.Description);
        transaction.ExpenseType.Should().Be(request.ExpenseType);
    }

    [Fact]
    public async Task Add_UpdatesUserBalance_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        User? userBeforeUpdate = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        TokenResponse tokenResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        AddExpenseRequest request = new()
        {
            Amount = 500,
            Currency = "BGN",
            Date = DateTime.UtcNow,
            Description = "Groceries - Store Name",
            ExpenseType = ExpenseType.Needs
        };

        HttpResponseMessage addExpenseResponse =
            await HttpClient.PostAsJsonAsync($"/api/finances/transactions/expense", request);

        addExpenseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        User? updatedUser = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        updatedUser.Should().NotBeNull();
        // TODO: this part works currently but the entire logic of balance
        // should be re-thinked, because there might be situations when user 
        // creates an expense transaction which is not with the same currency 
        // as his balance, so the conversion should implemented
        updatedUser.Balance.Amount.Should().Be(userBeforeUpdate.Balance.Amount - request.Amount);
    }
}