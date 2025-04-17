
using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Finances.Services.Contracts;

public interface IIncomeTransactionsManagement
{
    Task<DataResult<GetIncomeTransactionsResponse>> GetUserIncomesAsync(
        string userId,
        CancellationToken cancellationToken);
    Task<DataResult<Guid>> AddIncomeAsync(
        string userId,
        AddIncomeDto request,
        CancellationToken cancellationToken);
}
