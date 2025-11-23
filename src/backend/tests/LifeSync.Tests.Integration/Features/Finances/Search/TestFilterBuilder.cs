using LifeSync.API.Features.Finances.Search.Models;

namespace LifeSync.Tests.Integration.Features.Finances.TestQueryStringBuilder;

public static class TestFilterBuilder
{
    public static string Build(SearchTransactionsFilters filters)
    {
        List<string> queryParams = new();

        queryParams.Add($"description={Uri.EscapeDataString(filters.Description ?? string.Empty)}");

        if (filters.StartDate.HasValue)
        {
            queryParams.Add($"startDate={filters.StartDate.Value:yyyy-MM-dd}");
        }

        if (filters.EndDate.HasValue)
        {
            queryParams.Add($"endDate={filters.EndDate.Value:yyyy-MM-dd}");
        }

        if (filters.ExpenseTypes != null && filters.ExpenseTypes.Count > 0)
        {
            for (int i = 0; i < filters.ExpenseTypes.Count; i++)
            {
                queryParams.Add($"expenseTypes[{i}]={filters.ExpenseTypes[i]}");
            }
        }

        if (filters.TransactionTypes != null && filters.TransactionTypes.Count > 0)
        {
            for (int i = 0; i < filters.TransactionTypes.Count; i++)
            {
                queryParams.Add($"transactionTypes[{i}]={filters.TransactionTypes[i]}");
            }
        }

        return "?" + string.Join("&", queryParams);
    }
}