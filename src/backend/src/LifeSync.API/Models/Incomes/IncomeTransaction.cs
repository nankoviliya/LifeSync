using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.Incomes;

public class IncomeTransaction : Entity
{
    private const int MaxDescriptionLength = 500;
    private const int MinDescriptionLength = 1;

    private IncomeTransaction() { }

    public static IncomeTransaction From(
        RequiredReference<Money> amount,
        RequiredStruct<DateTime> date,
        RequiredString description,
        RequiredString userId)
    {
        Money amountValue = amount;
        DateTime dateValue = date;
        string descriptionValue = description;
        string userIdValue = userId;

        ValidateAmount(amountValue);
        ValidateDate(dateValue);
        ValidateDescription(descriptionValue);
        ValidateUserId(userIdValue);

        return new IncomeTransaction(amountValue, dateValue, descriptionValue.Trim(), userIdValue);
    }

    private IncomeTransaction(
        Money amount,
        DateTime date,
        string description,
        string userId)
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

    /// <summary>
    /// Updates the income amount
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
    /// Checks if the income occurred within a date range
    /// </summary>
    public bool IsWithinDateRange(DateTime startDate, DateTime endDate)
    {
        return Date >= startDate && Date <= endDate;
    }

    /// <summary>
    /// Checks if the income occurred in a specific month and year
    /// </summary>
    public bool IsInMonth(int year, int month)
    {
        return Date.Year == year && Date.Month == month;
    }

    /// <summary>
    /// Checks if the income occurred in a specific year
    /// </summary>
    public bool IsInYear(int year)
    {
        return Date.Year == year;
    }

    /// <summary>
    /// Calculates net income after deducting a percentage (e.g., for taxes)
    /// </summary>
    public Money CalculateNetIncome(decimal deductionPercentage)
    {
        if (deductionPercentage < 0 || deductionPercentage > 100)
        {
            throw new ArgumentException(
                "Deduction percentage must be between 0 and 100.",
                nameof(deductionPercentage));
        }

        var deduction = Amount * (deductionPercentage / 100);
        return Amount - deduction;
    }

    private static void ValidateAmount(Money amount)
    {
        if (amount == null)
        {
            throw new ArgumentNullException(nameof(amount), "Amount cannot be null.");
        }

        if (amount.IsNegative())
        {
            throw new ArgumentException("Income amount cannot be negative.", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Income amount must be greater than zero.", nameof(amount));
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
            throw new ArgumentException("Income date cannot be in the future.", nameof(date));
        }

        if (date < new DateTime(1900, 1, 1))
        {
            throw new ArgumentException("Income date is too far in the past.", nameof(date));
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

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }
    }

    public override string ToString() => $"Income: {Amount} - {Description} ({Date:yyyy-MM-dd})";
}