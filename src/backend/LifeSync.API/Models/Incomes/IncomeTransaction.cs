using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;

namespace LifeSync.API.Models.Incomes;

public class IncomeTransaction : Entity
{
    public Money Amount { get; init; } = default!;

    public DateTime Date { get; init; } 

    public string Description { get; init; } = default!; 

    public string UserId { get; init; } 

    public User User { get; init; } = default!; 
}