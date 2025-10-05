using FastEndpoints;
using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.Finances.Search;

// TODO: refactor frontend and return DataResult
// TODO: fix binding
public sealed class
    TransactionsSearchEndpoint : Endpoint<GetUserFinancialTransactionsRequest, GetUserFinancialTransactionsResponse>
{
    private readonly ITransactionsSearchService _transactionsSearchService;

    public TransactionsSearchEndpoint(ITransactionsSearchService transactionsSearchService) =>
        _transactionsSearchService = transactionsSearchService;

    public override void Configure()
    {
        Get("api/finances/transactions");

        Summary(s =>
        {
            s.Summary = "Retrieves financial transactions.";
            s.Description =
                "Returns object that contains a list of financial transactions for the authenticated user, filtered by query parameters.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(GetUserFinancialTransactionsRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<GetUserFinancialTransactionsResponse> result =
            await _transactionsSearchService.GetUserFinancialTransactionsAsync(userId, request, ct);

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