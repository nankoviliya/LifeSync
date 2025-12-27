using FluentAssertions;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Incomes;
using LifeSync.Tests.Integration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Finances.Incomes;

public class AddIncomeEndpointTests : IntegrationTestsBase
{
    public AddIncomeEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Add_AddsANewIncome_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        LoginResponse loginResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        AddIncomeRequest request = new()
        {
            Amount = 500, Currency = "BGN", Date = DateTime.UtcNow, Description = "Salary - August 2025"
        };

        HttpResponseMessage addIncomeResponse =
            await HttpClient.PostAsJsonAsync($"/api/finances/transactions/income", request);

        addIncomeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AddIncomeResponse? responseData = await addIncomeResponse.Content.ReadFromJsonAsync<AddIncomeResponse>();

        responseData.Should().NotBeNull();
        responseData.TransactionId.Should().NotBeEmpty();

        IncomeTransaction? transaction =
            await DbContext.IncomeTransactions.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == responseData.TransactionId);

        transaction.Should().NotBeNull();
        transaction.Amount.Amount.Should().Be(request.Amount);
        transaction.Amount.CurrencyCode.Should().Be(request.Currency);
        transaction.Date.Should().Be(request.Date);
        transaction.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task Add_UpdatesUserBalance_WhenRequestIsValid()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        User? userBeforeUpdate = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        LoginResponse loginResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        AddIncomeRequest request = new()
        {
            Amount = 500, Currency = "BGN", Date = DateTime.UtcNow, Description = "Salary - August 2025"
        };

        HttpResponseMessage addIncomeResponse =
            await HttpClient.PostAsJsonAsync($"/api/finances/transactions/income", request);

        addIncomeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        User? updatedUser = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == registerUserRequest.Email);

        updatedUser.Should().NotBeNull();
        // TODO: this part works currently but the entire logic of balance
        // should be re-thinked, because there might be situations when user 
        // creates an income transaction which is not with the same currency 
        // as his balance, so the conversion should implemented
        updatedUser.Balance.Amount.Should().Be(userBeforeUpdate.Balance.Amount + request.Amount);
    }
}