using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Expenses;

public class ExpenseTransaction : Entity
{
    private ExpenseTransaction() { }

    public static ExpenseTransaction From(
        RequiredReference<Money> amount,
        RequiredStruct<DateTime> date,
        RequiredString description,
        ExpenseType expenseType,
        RequiredString userId
    ) => new(amount, date, description, expenseType, userId);

    protected ExpenseTransaction(
        Money amount,
        DateTime date,
        string description,
        ExpenseType expenseType,
        string userId
    )
    {
        Amount = amount;
        Date = date;
        Description = description;
        ExpenseType = expenseType;
        UserId = userId;
    }

    public Money Amount { get; private set; } = default!;

    public DateTime Date { get; private set; }

    public string Description { get; private set; } = default!;

    public ExpenseType ExpenseType { get; private set; }

    public string UserId { get; private set; } = default!;

    public User User { get; private set; } = default!;
}

public enum ExpenseType
{
    Needs,
    Wants,
    Savings
}