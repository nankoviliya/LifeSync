using LifeSync.API.Features.IncomeTracking.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.IncomeTracking.Services;

public interface IIncomeTrackingService
{
    Task<DataResult<GetIncomeTransactionsResponse>> GetUserIncomesAsync(string userId);
    Task<DataResult<Guid>> AddIncomeAsync(string userId, AddIncomeDto request);
}