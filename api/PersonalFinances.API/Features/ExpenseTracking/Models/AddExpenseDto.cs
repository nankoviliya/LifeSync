using PersonalFinances.API.Models;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.ExpenseTracking.Models;

public class AddExpenseDto
{
    public Guid Id { get; init; } // Primary Key

    public Money Amount { get; init; } = default!; // Expense amount
    
    public DateTime Date { get; init; } // Date of expense

    public string Description { get; init; } = default!; // Optional details
    
    public ExpenseType ExpenseType { get; init; }
}