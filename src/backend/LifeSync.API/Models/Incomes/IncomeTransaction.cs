using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;

namespace LifeSync.API.Models.Incomes;

public class IncomeTransaction : Entity
{
    public Money Amount { get; init; } = default!;// Income amount

    public DateTime Date { get; init; } // Date of income

    public string Description { get; init; } = default!; // Optional details

    public Guid UserId { get; init; } // Foreign Key to User

    public User User { get; init; } = default!; // Owner of the transaction
}