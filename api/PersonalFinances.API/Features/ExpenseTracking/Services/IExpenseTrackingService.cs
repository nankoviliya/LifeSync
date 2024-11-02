using PersonalFinances.API.Features.ExpenseTracking.Models;

namespace PersonalFinances.API.Features.ExpenseTracking.Services;

public interface IExpenseTrackingService
{
    Task<IEnumerable<GetExpenseDto>> GetUserExpensesAsync(Guid userId);
    
    Task<Guid> AddExpenseAsync(Guid userId, AddExpenseDto request);
}