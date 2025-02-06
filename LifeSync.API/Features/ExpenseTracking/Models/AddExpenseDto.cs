using LifeSync.API.Models.Expenses;
using LifeSync.API.Shared;

namespace LifeSync.API.Features.ExpenseTracking.Models;

public class AddExpenseDto
{
    public decimal Amount { get; init; } = default!;
    
    public string Currency { get; init; } = default!;
    
    public DateTime Date { get; init; }

    public string Description { get; init; } = default!; 
    
    public ExpenseType ExpenseType { get; init; }
}