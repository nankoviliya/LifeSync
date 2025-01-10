using PersonalFinances.API.Models.Abstractions;
using PersonalFinances.API.Models.ApplicationUser;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Models;

public class IncomeTransaction : Entity
{
    public Money Amount { get; init; } = default!;// Income amount
    
    public DateTime Date { get; init; } // Date of income

    public string Description { get; init; } = default!; // Optional details
    
    public Guid UserId { get; init; } // Foreign Key to User

    public User User { get; init; } = default!; // Owner of the transaction
}