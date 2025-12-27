using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Models.Expenses;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Finances;

public static class FinanceTestsHelper
{
    public static AddIncomeRequest DefaultAddIncomeRequest = new()
    {
        Amount = 500, Currency = "BGN", Date = DateTime.UtcNow, Description = "Salary - August 2025"
    };

    public static AddExpenseRequest DefaultAddExpenseRequest = new()
    {
        Amount = 500,
        Currency = "BGN",
        Date = DateTime.UtcNow,
        Description = "Groceries - Store Name",
        ExpenseType = ExpenseType.Needs
    };

    public static async Task<HttpResponseMessage> AddIncomeTransaction(this HttpClient httpClient,
        AddIncomeRequest addIncomeRequest,
        string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync($"/api/finances/transactions/income", addIncomeRequest);

        return response;
    }

    public static async Task<HttpResponseMessage> AddExpenseTransaction(this HttpClient httpClient,
        AddExpenseRequest addExpenseRequest,
        string accessToken)
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync($"/api/finances/transactions/expense", addExpenseRequest);

        return response;
    }
}