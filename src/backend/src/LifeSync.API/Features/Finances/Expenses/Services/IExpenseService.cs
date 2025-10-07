using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Finances.Expenses.Services;

public interface IExpenseService
{
    Task<DataResult<Guid>> AddExpenseAsync(
        RequiredString userId,
        AddExpenseRequest request,
        CancellationToken cancellationToken);
}
