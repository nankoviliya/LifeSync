using LifeSync.API.Models.Abstractions;
using LifeSync.API.Shared;
using LifeSync.Common.Required;

namespace LifeSync.API.Models.ApplicationUser;

public class UserWallet : Entity
{
    private UserWallet() { }

    public static UserWallet From(
        RequiredString userId,
        RequiredString name,
        RequiredReference<Money> initialBalance)
    {
        ValidateName(name);
        ValidateUserId(userId);

        Money balanceValue = initialBalance;
        if (balanceValue.Amount < 0)
        {
            throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));
        }

        string userIdValue = userId;
        string nameValue = name;
        return new UserWallet(userIdValue, nameValue.Trim(), balanceValue);
    }

    private UserWallet(string userId, string name, Money balance)
    {
        UserId = userId;
        Name = name;
        Balance = balance;
    }

    public string UserId { get; private set; } = default!;

    public User User { get; init; } = default!;

    public string Name { get; private set; } = default!;

    public Money Balance { get; private set; } = default!;

    /// <summary>
    /// Updates the wallet name
    /// </summary>
    public void Rename(RequiredString newName)
    {
        ValidateName(newName);
        string value = newName;
        Name = value.Trim();
    }

    /// <summary>
    /// Deposits money into the wallet
    /// </summary>
    public void Deposit(Money amount)
    {
        if (amount.IsNegative())
        {
            throw new ArgumentException("Cannot deposit negative amount.", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Cannot deposit zero amount.", nameof(amount));
        }

        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot deposit {amount.CurrencyCode} into wallet with {Balance.CurrencyCode}",
                nameof(amount));
        }

        Balance += amount;
    }

    /// <summary>
    /// Withdraws money from the wallet
    /// </summary>
    public void Withdraw(Money amount)
    {
        if (amount.IsNegative())
        {
            throw new ArgumentException("Cannot withdraw negative amount.", nameof(amount));
        }

        if (amount.IsZero())
        {
            throw new ArgumentException("Cannot withdraw zero amount.", nameof(amount));
        }

        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot withdraw {amount.CurrencyCode} from wallet with {Balance.CurrencyCode}",
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
    /// Transfers money to another wallet
    /// </summary>
    public void TransferTo(UserWallet targetWallet, Money amount)
    {
        if (targetWallet == null)
        {
            throw new ArgumentNullException(nameof(targetWallet));
        }

        if (targetWallet == this)
        {
            throw new InvalidOperationException("Cannot transfer to the same wallet.");
        }

        if (targetWallet.IsDeleted)
        {
            throw new InvalidOperationException("Cannot transfer to a deleted wallet.");
        }

        if (amount.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot transfer {amount.CurrencyCode} from wallet with {Balance.CurrencyCode}",
                nameof(amount));
        }

        if (amount.CurrencyCode != targetWallet.Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot transfer {amount.CurrencyCode} to wallet with {targetWallet.Balance.CurrencyCode}",
                nameof(amount));
        }

        Withdraw(amount);
        targetWallet.Deposit(amount);
    }

    /// <summary>
    /// Updates the wallet balance directly (for corrections or adjustments)
    /// </summary>
    public void SetBalance(Money newBalance)
    {
        if (newBalance.CurrencyCode != Balance.CurrencyCode)
        {
            throw new ArgumentException(
                $"Currency mismatch: Cannot set balance to {newBalance.CurrencyCode} when wallet currency is {Balance.CurrencyCode}",
                nameof(newBalance));
        }

        Balance = newBalance;
    }

    /// <summary>
    /// Checks if the wallet has sufficient balance
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
    /// Checks if the wallet is empty
    /// </summary>
    public bool IsEmpty() => Balance.IsZero();

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Wallet name cannot be null or empty.", nameof(name));
        }

        if (name.Trim().Length < 2)
        {
            throw new ArgumentException("Wallet name must be at least 2 characters.", nameof(name));
        }

        if (name.Trim().Length > 100)
        {
            throw new ArgumentException("Wallet name cannot exceed 100 characters.", nameof(name));
        }
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }
    }

    public override string ToString() => $"{Name}: {Balance}";
}