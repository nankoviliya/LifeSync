using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Finances.Incomes.Services;

public interface IIncomeService
{
    Task<DataResult<Guid>> AddIncomeAsync(
        RequiredString userId,
        AddIncomeRequest request,
        CancellationToken cancellationToken);
}
