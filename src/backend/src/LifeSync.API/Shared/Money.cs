namespace LifeSync.API.Shared;

public record Money : IComparable<Money>
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    private Money()
    {
        Amount = 0;
        Currency = Currency.None;
    }

    public Money(decimal amount, Currency currency)
    {
        if (currency == Currency.None && amount != 0)
        {
            throw new ArgumentException("Amount must be zero when currency is None.", nameof(amount));
        }

        Amount = Math.Round(amount, 2);
        Currency = currency;
    }

    public static Money operator +(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "add");
        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "subtract");
        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator *(decimal multiplier, Money money)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("Cannot divide money by zero.");
        }

        return new Money(money.Amount / divisor, money.Currency);
    }

    public static bool operator >(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "compare");
        return first.Amount > second.Amount;
    }

    public static bool operator <(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "compare");
        return first.Amount < second.Amount;
    }

    public static bool operator >=(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "compare");
        return first.Amount >= second.Amount;
    }

    public static bool operator <=(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "compare");
        return first.Amount <= second.Amount;
    }

    public static Money Zero() => new Money(0, Currency.None);

    public static Money Zero(Currency currency) => new Money(0, currency);

    public bool IsZero() => Amount == 0;

    public bool IsPositive() => Amount > 0;

    public bool IsNegative() => Amount < 0;

    public Money Abs() => new Money(Math.Abs(Amount), Currency);

    public Money Negate() => new Money(-Amount, Currency);

    /// <summary>
    /// Allocates money into parts according to given ratios
    /// </summary>
    /// <param name="ratios">Ratios to allocate by (e.g., [50, 30, 20] for 50%, 30%, 20%)</param>
    /// <returns>Array of Money objects allocated according to ratios</returns>
    public Money[] Allocate(params int[] ratios)
    {
        if (ratios == null || ratios.Length == 0)
        {
            throw new ArgumentException("Ratios cannot be null or empty.", nameof(ratios));
        }

        if (ratios.Any(r => r < 0))
        {
            throw new ArgumentException("Ratios cannot be negative.", nameof(ratios));
        }

        var total = ratios.Sum();
        if (total == 0)
        {
            throw new ArgumentException("Sum of ratios cannot be zero.", nameof(ratios));
        }

        var result = new Money[ratios.Length];
        var remainder = Amount;

        for (int i = 0; i < ratios.Length; i++)
        {
            var share = Math.Round(Amount * ratios[i] / total, 2);
            result[i] = new Money(share, Currency);
            remainder -= share;
        }

        // Add any remainder to the first allocation to handle rounding
        if (remainder != 0 && ratios.Length > 0)
        {
            result[0] = new Money(result[0].Amount + remainder, Currency);
        }

        return result;
    }

    /// <summary>
    /// Converts money to a different currency (requires exchange rate)
    /// </summary>
    public Money ConvertTo(Currency targetCurrency, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
        {
            throw new ArgumentException("Exchange rate must be positive.", nameof(exchangeRate));
        }

        if (Currency == targetCurrency)
        {
            return this;
        }

        return new Money(Amount * exchangeRate, targetCurrency);
    }

    public int CompareTo(Money? other)
    {
        if (other is null)
        {
            return 1;
        }

        ValidateSameCurrency(this, other, "compare");
        return Amount.CompareTo(other.Amount);
    }

    private static void ValidateSameCurrency(Money first, Money second, string operation)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException(
                $"Cannot {operation} money with different currencies: {first.Currency} and {second.Currency}");
        }
    }

    public override string ToString() => $"{Amount:N2} {Currency}";
}