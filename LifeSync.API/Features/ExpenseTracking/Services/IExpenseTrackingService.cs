using LifeSync.API.Features.ExpenseTracking.Models;

namespace LifeSync.API.Features.ExpenseTracking.Services;

public interface IExpenseTrackingService
{
    Task<IEnumerable<GetExpenseDto>> GetUserExpensesAsync(Guid userId);
    
    Task<Guid> AddExpenseAsync(Guid userId, AddExpenseDto request);
}