
using LifeSync.API.Features.Finances.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Finances.Services.Contracts;

public interface IExpenseTransactionsManagement
{
    Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpenseTransactionsAsync(
        Guid userId,
        GetUserExpenseTransactionsRequest request,
        CancellationToken cancellationToken);

    Task<DataResult<Guid>> AddExpenseAsync(
        Guid userId,
        AddExpenseDto request,
        CancellationToken cancellationToken);
}
