using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.API.Features.Finances.Shared.Models;
using LifeSync.API.Models.Expenses;
using LifeSync.Tests.Integration.Features.Finances.TestQueryStringBuilder;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    public async Task Search_ShouldReturnTargetElements_WhenFiltersAreWorkingCorrectly()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        TokenResponse tokenResponse = await LoginUserAsync(registerUserRequest.Email, registerUserRequest.Password);

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

        SearchTransactionsFilters filters = new()
        {
            TransactionTypes = [TransactionType.Expense, TransactionType.Income]
        };

        string queryString = TestFilterBuilder.Build(filters);
        HttpResponseMessage searchTransactionsResponse =
            await HttpClient.GetAsync($"/api/finances/transactions{queryString}");

        searchTransactionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        SearchTransactionsResponse? responseData =
            await searchTransactionsResponse.Content.ReadFromJsonAsync<SearchTransactionsResponse>(
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() }
                });

        responseData.Should().NotBeNull();
        responseData.TransactionsCount.Should().BeGreaterThan(0);
        responseData.Transactions.Count.Should().BeGreaterThan(0);
        responseData.ExpenseSummary.Should().NotBeNull();
        responseData.IncomeSummary.Should().NotBeNull();
    }
}