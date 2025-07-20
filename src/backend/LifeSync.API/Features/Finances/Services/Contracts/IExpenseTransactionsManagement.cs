
using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Finances.Services.Contracts;

public interface IExpenseTransactionsManagement
{
    Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpenseTransactionsAsync(
        string userId,
        GetUserExpenseTransactionsRequest request,
        CancellationToken cancellationToken);

    Task<DataResult<Guid>> AddExpenseAsync(
        string userId,
        AddExpenseDto request,
        CancellationToken cancellationToken);
}
