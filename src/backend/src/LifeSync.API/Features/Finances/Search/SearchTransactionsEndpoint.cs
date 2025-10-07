using FastEndpoints;
using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.API.Features.Finances.Search.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.Finances.Search;

public sealed class SearchTransactionsEndpoint : Endpoint<SearchTransactionsRequest, SearchTransactionsResponse>
{
    private readonly ITransactionsSearchService _transactionsSearchService;

    public SearchTransactionsEndpoint(ITransactionsSearchService transactionsSearchService) =>
        _transactionsSearchService = transactionsSearchService;

    public override void Configure()
    {
        Get("api/finances/transactions");

        Summary(s =>
        {
            s.Summary = "Search financial transactions";
            s.Description =
                "Returns a list of financial transactions for the authenticated user, filtered by query parameters.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(SearchTransactionsRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<SearchTransactionsResponse> result =
            await _transactionsSearchService.SearchTransactionsAsync(userId, request, ct);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(400, ct);
        }
        else
        {
            await SendOkAsync(result.Data, ct);
        }
    }
}
