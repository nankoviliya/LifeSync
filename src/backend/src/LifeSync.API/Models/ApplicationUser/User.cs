using LifeSync.API.Models.Expenses;
using LifeSync.API.Models.Incomes;
using LifeSync.API.Models.Languages;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Models.ApplicationUser;

public class User : IdentityUser
{
    private User() { }

    public static User From(
        RequiredString userName,
        RequiredString email,
        RequiredString firstName,
        RequiredString lastName,
        RequiredReference<Money> balance,
        RequiredReference<Currency> currencyPreference,
        RequiredStruct<Guid> languageId)
    {
        User user = new();
        user.Initialize(
            userName,
            email,
            firstName,
            lastName,
            balance,
            currencyPreference,
            languageId);

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
        Currency currencyPreference,
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

    public Currency CurrencyPreference { get; private set; } = default!;

    public Guid LanguageId { get; private set; }

    public Language Language { get; init; } = default!;

    public ICollection<IncomeTransaction> IncomeTransactions { get; init; } = [];

    public ICollection<ExpenseTransaction> ExpenseTransactions { get; init; } = [];

    /// <summary>
    /// Updates user's first name
    /// </summary>
    /// <param name="firstName">New first name of the user</param>
    public void UpdateFirstName(RequiredString firstName) => FirstName = firstName;

    /// <summary>
    /// Updates user's last name
    /// </summary>
    /// <param name="lastName">New last name of the user</param>
    public void UpdateLastName(RequiredString lastName) => LastName = lastName;


    /// <summary>
    /// Updates the user's preferred language
    /// </summary>
    /// <param name="languageId">The ID of the new language</param>
    public void UpdateLanguage(RequiredStruct<Guid> languageId) => LanguageId = languageId;

    /// <summary>
    /// Updates the user's balance to a new value
    /// </summary>
    /// <param name="newBalance">The new balance amount</param>
    /// <exception cref="ArgumentException">Thrown when currency doesn't match user's currency preference</exception>
    public void UpdateBalance(Money newBalance)
    {
        if (newBalance.Currency != CurrencyPreference)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot set balance to {newBalance.Currency} when user's currency preference is {CurrencyPreference}",
                nameof(newBalance));
        }

        Balance = newBalance;
    }

    /// <summary>
    /// Deposits money into the user's balance
    /// </summary>
    /// <param name="amount">The amount to deposit</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative or has different currency</exception>
    public void Deposit(Money amount)
    {
        if (amount.Amount < 0)
        {
            throw new ArgumentException("Cannot record negative income amount", nameof(amount));
        }

        if (amount.Currency != Balance.Currency)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot add {amount.Currency} to balance in {Balance.Currency}",
                nameof(amount));
        }

        Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
    }

    /// <summary>
    /// Withdraws money from the user's balance
    /// </summary>
    /// <param name="amount">The amount to withdraw</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative or has different currency</exception>
    /// <exception cref="InvalidOperationException">Thrown when balance is insufficient</exception>
    public void Withdraw(Money amount)
    {
        if (amount.Amount < 0)
        {
            throw new ArgumentException("Cannot withdraw negative amount", nameof(amount));
        }

        if (amount.Currency != Balance.Currency)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot withdraw {amount.Currency} from balance in {Balance.Currency}",
                nameof(amount));
        }

        if (Balance.Amount < amount.Amount)
        {
            throw new InvalidOperationException(
                $"Insufficient balance: Cannot withdraw {amount.Amount} from balance of {Balance.Amount}");
        }

        Balance = new Money(Balance.Amount - amount.Amount, Balance.Currency);
    }
}