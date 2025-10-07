using LifeSync.API.Features.Finances.Search.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Finances.Search.Services;

public interface ITransactionsSearchService
{
    Task<DataResult<SearchTransactionsResponse>> SearchTransactionsAsync(
        RequiredString userId,
        SearchTransactionsRequest request,
        CancellationToken cancellationToken);
}
