using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Incomes;

public class IncomeTransaction : Entity
{
    private IncomeTransaction() { }

    public static IncomeTransaction From(
        RequiredReference<Money> amount,
        RequiredStruct<DateTime> date,
        RequiredString description,
        RequiredString userId
    ) => new(amount, date, description, userId);

    protected IncomeTransaction(
        Money amount,
        DateTime date,
        string description,
        string userId
    )
    {
        Amount = amount;
        Date = date;
        Description = description;
        UserId = userId;
    }

    public Money Amount { get; init; } = default!;

    public DateTime Date { get; init; }

    public string Description { get; init; } = default!;

    public string UserId { get; init; } = default!;

    public User User { get; init; } = default!;
}