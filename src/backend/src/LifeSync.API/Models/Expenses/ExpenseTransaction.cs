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
    Savings
}