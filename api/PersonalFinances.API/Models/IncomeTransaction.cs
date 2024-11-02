using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Models;

public class IncomeTransaction
{
    public Guid Id { get; init; } // Primary Key

    public Money Amount { get; init; } = default!;// Income amount
    
    public DateTime Date { get; init; } // Date of income

    public string Description { get; init; } = default!; // Optional details
    
    public int UserId { get; init; } // Foreign Key to User

    public User User { get; init; } = default!; // Owner of the transaction
}