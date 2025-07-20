using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Finances.Services.Contracts;

public interface ITransactionsSearchService
{
    Task<DataResult<GetUserFinancialTransactionsResponse>> GetUserFinancialTransactionsAsync(
        string userId,
        GetUserFinancialTransactionsRequest request,
        CancellationToken cancellationToken);
}
