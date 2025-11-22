using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Finances.Search;

public class SearchTransactionsEndpointTests : IntegrationTestsBase
{
    public SearchTransactionsEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    protected override async Task OnInitializeAsync()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        TokenResponse tokenResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        await HttpClient.AddExpenseTransaction(FinanceTestsHelper.DefaultAddExpenseRequest, tokenResponse);
        await HttpClient.AddIncomeTransaction(FinanceTestsHelper.DefaultAddIncomeRequest, tokenResponse);
    }

    [Fact]
    public async Task Search_ShouldReturnTargetObjects_WhenFiltersAreWorkingCorrectly()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        TokenResponse tokenResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        // TODO: implement builder for 'SearchTransactionsRequest' class and test 
        // different search scenarios
        string queryString =
            "?description=&expenseTypes[0]=Needs&expenseTypes[1]=Wants&expenseTypes[2]=Savings&transactionTypes[0]=Expense&transactionTypes[1]=Income";

        HttpResponseMessage searchTransactionsResponse =
            await HttpClient.GetAsync($"/api/finances/transactions{queryString}");

        searchTransactionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // TODO: solve the problem with deserialization
        SearchTransactionsResponse? responseData =
            await searchTransactionsResponse.Content.ReadFromJsonAsync<SearchTransactionsResponse>();

        responseData.Should().NotBeNull();
        responseData.TransactionsCount.Should().Be(2);
        responseData.Transactions.Should().HaveCount(2);
        responseData.ExpenseSummary.Should().NotBeNull();
        responseData.IncomeSummary.Should().NotBeNull();
    }
}