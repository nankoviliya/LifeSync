using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Languages;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace LifeSync.API.Models.ApplicationUser;

public class User : IdentityUser
{
    private const int MinNameLength = 1;
    private const int MaxNameLength = 100;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(500));

    private User() { }

    public static User From(
        RequiredString userName,
        RequiredString email,
        RequiredString firstName,
        RequiredString lastName,
        RequiredReference<Money> balance,
        RequiredString currencyPreference,
        RequiredStruct<Guid> languageId)
    {
        string userNameValue = userName;
        string emailValue = email;
        string firstNameValue = firstName;
        string lastNameValue = lastName;
        Money balanceValue = balance;
        string currencyValue = currencyPreference;
        Guid languageIdValue = languageId;

        ValidateUserName(userNameValue);
        ValidateEmail(emailValue);
        ValidateFirstName(firstNameValue);
        ValidateLastName(lastNameValue);
        ValidateBalance(balanceValue);
        ValidateCurrencyPreference(currencyValue);
        ValidateLanguageId(languageIdValue);

        User user = new();
        user.Initialize(
            userNameValue.Trim(),
            emailValue.Trim(),
            firstNameValue.Trim(),
            lastNameValue.Trim(),
            balanceValue,
            currencyValue.ToUpperInvariant().Trim(),
            languageIdValue);

        return user;
    }

    /// <summary>
    /// Internal initialization logic separated from the constructor.
    /// This method can safely set up the object's full state without
    /// risking partially constructed object access.
    /// </summary>
    private void Initialize(
        string userName,
        string email,
        string firstName,
        string lastName,
        Money balance,
        string currencyPreference,
        Guid languageId)
    {
        UserName = userName;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Balance = balance;
        CurrencyPreference = currencyPreference;
        LanguageId = languageId;
    }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public Money Balance { get; private set; } = default!;

    public string CurrencyPreference { get; private set; } = default!;

    public Guid LanguageId { get; private set; }

    public Language Language { get; init; } = default!;

    public ICollection<IncomeTransaction> IncomeTransactions { get; init; } = [];

    public ICollection<ExpenseTransaction> ExpenseTransactions { get; init; } = [];

    /// <summary>
    /// Gets the user's full name
    /// </summary>
    public string GetFullName() => $"{FirstName} {LastName}";

    /// <summary>
    /// Updates user's first name
    /// </summary>
    public void UpdateFirstName(RequiredString firstName)
    {
        string value = firstName;
        ValidateFirstName(value);
        FirstName = value.Trim();
    }

    /// <summary>
    /// Updates user's last name
    /// </summary>
    public void UpdateLastName(RequiredString lastName)
    {
        string value = lastName;
        ValidateLastName(value);
        LastName = value.Trim();
    }

    /// <summary>
    /// Updates both first and last names
    /// </summary>
    public void UpdateFullName(RequiredString firstName, RequiredString lastName)
    {
        string firstNameValue = firstName;
        string lastNameValue = lastName;
        ValidateFirstName(firstNameValue);
        ValidateLastName(lastNameValue);
        FirstName = firstNameValue.Trim();
        LastName = lastNameValue.Trim();
    }

    /// <summary>
    /// Updates the user's preferred language
    /// </summary>
    public void UpdateLanguage(RequiredStruct<Guid> languageId)
    {
        ValidateLanguageId(languageId);
        LanguageId = languageId;
    }

    /// <summary>
    /// Updates the user's currency preference and converts balance
    /// </summary>
    public void UpdateCurrencyPreference(RequiredString newCurrency, decimal exchangeRate)
    {
        string newCurrencyValue = newCurrency;
        ValidateCurrencyPreference(newCurrencyValue);

        if (exchangeRate <= 0)
        {
            throw new ArgumentException("Exchange rate must be positive.", nameof(exchangeRate));
        }

        string normalizedCurrency = newCurrencyValue.ToUpperInvariant().Trim();

        if (normalizedCurrency != CurrencyPreference)
        {
            Balance = Balance.ConvertTo(normalizedCurrency, exchangeRate);
            CurrencyPreference = normalizedCurrency;
        }
    }

    /// <summary>
    /// Updates the user's balance to a new value
    /// </summary>
    public void UpdateBalance(Money newBalance)
    {
        if (newBalance == null)
        {
            throw new ArgumentNullException(nameof(newBalance), "Balance cannot be null.");
        }

        if (newBalance.CurrencyCode != CurrencyPreference)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot set balance to {newBalance.CurrencyCode} when user's currency preference is {CurrencyPreference}",
                nameof(newBalance));
        }

        Balance = newBalance;
    }

    /// <summary>
    /// Deposits money into the user's balance
    /// </summary>
    public void Deposit(Money amount)
    {
        if (amount.IsNegative())
        {
            throw new ArgumentException("Cannot deposit negative amount", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Cannot deposit zero amount", nameof(amount));
        }

        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot add {amount.CurrencyCode} to balance in {Balance.CurrencyCode}",
                nameof(amount));
        }

        Balance += amount;
    }

    /// <summary>
    /// Withdraws money from the user's balance
    /// </summary>
    public void Withdraw(Money amount)
    {
        if (amount.IsNegative())
        {
            throw new ArgumentException("Cannot withdraw negative amount", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Cannot withdraw zero amount", nameof(amount));
        }

        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot withdraw {amount.CurrencyCode} from balance in {Balance.CurrencyCode}",
                nameof(amount));
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException(
                $"Insufficient balance: Cannot withdraw {amount} from balance of {Balance}");
        }

        Balance -= amount;
    }

    /// <summary>
    /// Checks if the user has sufficient balance
    /// </summary>
    public bool HasSufficientBalance(Money amount)
    {
        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            return false;
        }

        return Balance >= amount;
    }

    /// <summary>
    /// Gets total income for a specific period
    /// </summary>
    public Money GetTotalIncome(DateTime startDate, DateTime endDate)
    {
        Money total = Money.Zero(CurrencyPreference);

        foreach (IncomeTransaction income in IncomeTransactions.Where(i => i.IsWithinDateRange(startDate, endDate)))
        {
            total += income.Amount;
        }

        return total;
    }

    /// <summary>
    /// Gets total expenses for a specific period
    /// </summary>
    public Money GetTotalExpenses(DateTime startDate, DateTime endDate)
    {
        Money total = Money.Zero(CurrencyPreference);

        foreach (ExpenseTransaction expense in ExpenseTransactions.Where(e => e.IsWithinDateRange(startDate, endDate)))
        {
            total += expense.Amount;
        }

        return total;
    }

    /// <summary>
    /// Gets net income (income - expenses) for a specific period
    /// </summary>
    public Money GetNetIncome(DateTime startDate, DateTime endDate)
    {
        Money totalIncome = GetTotalIncome(startDate, endDate);
        Money totalExpenses = GetTotalExpenses(startDate, endDate);
        return totalIncome - totalExpenses;
    }

    /// <summary>
    /// Gets total expenses by type for a specific period
    /// </summary>
    public Money GetExpensesByType(ExpenseType expenseType, DateTime startDate, DateTime endDate)
    {
        Money total = Money.Zero(CurrencyPreference);

        foreach (ExpenseTransaction expense in ExpenseTransactions
                     .Where(e => e.ExpenseType == expenseType && e.IsWithinDateRange(startDate, endDate)))
        {
            total += expense.Amount;
        }

        return total;
    }

    private static void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(userName));
        }

        if (userName.Trim().Length < 3)
        {
            throw new ArgumentException("Username must be at least 3 characters.", nameof(userName));
        }

        if (userName.Trim().Length > 50)
        {
            throw new ArgumentException("Username cannot exceed 50 characters.", nameof(userName));
        }
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        }

        if (!EmailRegex.IsMatch(email.Trim()))
        {
            throw new ArgumentException("Email format is invalid.", nameof(email));
        }
    }

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
        }

        int trimmedLength = firstName.Trim().Length;

        if (trimmedLength < MinNameLength)
        {
            throw new ArgumentException(
                $"First name must be at least {MinNameLength} character(s).",
                nameof(firstName));
        }

        if (trimmedLength > MaxNameLength)
        {
            throw new ArgumentException(
                $"First name cannot exceed {MaxNameLength} characters.",
                nameof(firstName));
        }
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
        }

        int trimmedLength = lastName.Trim().Length;

        if (trimmedLength < MinNameLength)
        {
            throw new ArgumentException(
                $"Last name must be at least {MinNameLength} character(s).",
                nameof(lastName));
        }

        if (trimmedLength > MaxNameLength)
        {
            throw new ArgumentException(
                $"Last name cannot exceed {MaxNameLength} characters.",
                nameof(lastName));
        }
    }

    private static void ValidateBalance(Money balance)
    {
        if (balance == null)
        {
            throw new ArgumentNullException(nameof(balance), "Balance cannot be null.");
        }
    }

    private static void ValidateCurrencyPreference(string currencyPreference)
    {
        if (string.IsNullOrWhiteSpace(currencyPreference))
        {
            throw new ArgumentException("Currency preference cannot be null or empty.", nameof(currencyPreference));
        }

        if (!CurrencyRegistry.IsSupported(currencyPreference))
        {
            throw new ArgumentException("Currency is not supported.", nameof(currencyPreference));
        }
    }

    private static void ValidateLanguageId(Guid languageId)
    {
        if (languageId == Guid.Empty)
        {
            throw new ArgumentException("Language ID cannot be empty.", nameof(languageId));
        }
    }
}