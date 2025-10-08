using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Expenses;

public class ExpenseTransaction : Entity
{
    private const int MaxDescriptionLength = 500;
    private const int MinDescriptionLength = 1;

    private ExpenseTransaction() { }

    public static ExpenseTransaction From(
        RequiredReference<Money> amount,
        RequiredStruct<DateTime> date,
        RequiredString description,
        ExpenseType expenseType,
        RequiredString userId)
    {
        Money amountValue = amount;
        DateTime dateValue = date;
        string descriptionValue = description;
        string userIdValue = userId;

        ValidateAmount(amountValue);
        ValidateDate(dateValue);
        ValidateDescription(descriptionValue);
        ValidateExpenseType(expenseType);
        ValidateUserId(userIdValue);

        return new ExpenseTransaction(amountValue, dateValue, descriptionValue.Trim(), expenseType, userIdValue);
    }

    private ExpenseTransaction(
        Money amount,
        DateTime date,
        string description,
        ExpenseType expenseType,
        string userId)
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

    /// <summary>
    /// Updates the expense amount
    /// </summary>
    public void UpdateAmount(RequiredReference<Money> newAmount)
    {
        ValidateAmount(newAmount);
        Amount = newAmount;
    }

    /// <summary>
    /// Updates the transaction date
    /// </summary>
    public void UpdateDate(RequiredStruct<DateTime> newDate)
    {
        ValidateDate(newDate);
        Date = newDate;
    }

    /// <summary>
    /// Updates the description
    /// </summary>
    public void UpdateDescription(RequiredString newDescription)
    {
        string value = newDescription;
        ValidateDescription(value);
        Description = value.Trim();
    }

    /// <summary>
    /// Changes the expense type
    /// </summary>
    public void ChangeExpenseType(ExpenseType newExpenseType)
    {
        ValidateExpenseType(newExpenseType);
        ExpenseType = newExpenseType;
    }

    /// <summary>
    /// Checks if this is a need expense
    /// </summary>
    public bool IsNeed() => ExpenseType == ExpenseType.Needs;

    /// <summary>
    /// Checks if this is a want expense
    /// </summary>
    public bool IsWant() => ExpenseType == ExpenseType.Wants;

    /// <summary>
    /// Checks if this is a savings expense
    /// </summary>
    public bool IsSavings() => ExpenseType == ExpenseType.Savings;

    /// <summary>
    /// Checks if the expense occurred within a date range
    /// </summary>
    public bool IsWithinDateRange(DateTime startDate, DateTime endDate)
    {
        return Date >= startDate && Date <= endDate;
    }

    /// <summary>
    /// Checks if the expense occurred in a specific month and year
    /// </summary>
    public bool IsInMonth(int year, int month)
    {
        return Date.Year == year && Date.Month == month;
    }

    /// <summary>
    /// Checks if the expense occurred in a specific year
    /// </summary>
    public bool IsInYear(int year)
    {
        return Date.Year == year;
    }

    private static void ValidateAmount(Money amount)
    {
        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount), "Amount cannot be null.");
        }

        if (amount.IsNegative())
        {
            throw new ArgumentException("Expense amount cannot be negative.", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Expense amount must be greater than zero.", nameof(amount));
        }
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == default)
        {
            throw new ArgumentException("Date cannot be default value.", nameof(date));
        }

        if (date > DateTime.UtcNow.AddDays(1))
        {
            throw new ArgumentException("Expense date cannot be in the future.", nameof(date));
        }

        if (date < new DateTime(1900, 1, 1))
        {
            throw new ArgumentException("Expense date is too far in the past.", nameof(date));
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }

        var trimmedLength = description.Trim().Length;

        if (trimmedLength < MinDescriptionLength)
        {
            throw new ArgumentException(
                $"Description must be at least {MinDescriptionLength} character(s).",
                nameof(description));
        }

        if (trimmedLength > MaxDescriptionLength)
        {
            throw new ArgumentException(
                $"Description cannot exceed {MaxDescriptionLength} characters.",
                nameof(description));
        }
    }

    private static void ValidateExpenseType(ExpenseType expenseType)
    {
        if (!Enum.IsDefined(typeof(ExpenseType), expenseType))
        {
            throw new ArgumentException("Invalid expense type.", nameof(expenseType));
        }
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }
    }

    public override string ToString() => $"{ExpenseType}: {Amount} - {Description} ({Date:yyyy-MM-dd})";
}

public enum ExpenseType
{
    Needs,
    Wants,
    Savings
}