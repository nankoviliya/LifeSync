using LifeSync.API.Features.ExpenseTracking.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.ExpenseTracking.Services;

public interface IExpenseTrackingService
{
    Task<DataResult<GetExpenseTransactionsResponse>> GetUserExpensesAsync(string userId);

    Task<DataResult<Guid>> AddExpenseAsync(string userId, AddExpenseDto request);
}