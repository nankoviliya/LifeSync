using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;

namespace LifeSync.API.Models.Expenses;

public class ExpenseTransaction : Entity
{
    public Money Amount { get; init; } = default!;

    public DateTime Date { get; init; }

    public string Description { get; init; } = default!;

    public ExpenseType ExpenseType { get; init; }

    public string UserId { get; init; } = default!;

    public User User { get; init; } = default!; 
}

public enum ExpenseType
{
    Needs,
    Wants,
    Savings,
}