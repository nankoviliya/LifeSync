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

    public Money Amount { get; private set; } = default!;

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = default!;

    public string UserId { get; private set; } = default!;

    public User User { get; private set; } = default!;
}