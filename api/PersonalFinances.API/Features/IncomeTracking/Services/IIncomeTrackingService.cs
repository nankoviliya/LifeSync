using PersonalFinances.API.Features.IncomeTracking.Models;

namespace PersonalFinances.API.Features.IncomeTracking.Services;

public interface IIncomeTrackingService
{
    Task<IEnumerable<GetIncomeDto>> GetUserIncomesAsync(Guid userId);
    Task<Guid> AddIncomeAsync(Guid userId, AddIncomeDto request);
}