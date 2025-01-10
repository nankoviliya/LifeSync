using PersonalFinances.API.Models.Abstractions;
using PersonalFinances.API.Models.ApplicationUser;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Models;

public class ExpenseTransaction : Entity
{
    public Money Amount { get; init; } = default!; // Expense amount
    
    public DateTime Date { get; init; } // Date of expense

    public string Description { get; init; } = default!; // Optional details
    
    public ExpenseType ExpenseType { get; init; }
    
    public Guid UserId { get; init; } // Foreign Key to User

    public User User { get; init; } = default!; // Owner of the transaction
}

public enum ExpenseType
{
    Needs,
    Wants,
    Savings,
}