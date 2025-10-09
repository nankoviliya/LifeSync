namespace LifeSync.API.Shared;

public record Money : IComparable<Money>
{
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; }

    private Money()
    {
        Amount = 0;
        CurrencyCode = string.Empty;
    }

    public Money(decimal amount, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ArgumentException("Currency code cannot be null or empty.", nameof(currencyCode));
        }

        if (currencyCode.Length != 3)
        {
            throw new ArgumentException("Currency code must be 3 characters (ISO 4217 format).", nameof(currencyCode));
        }

        if (!IsValidIsoCurrencyFormat(currencyCode))
        {
            throw new ArgumentException(
                "Currency code must contain only letters (ISO 4217 format).",
                nameof(currencyCode));
        }

        var normalizedCode = currencyCode.ToUpperInvariant();

        // Validate against CurrencyRegistry to enforce business rule: only supported currencies can exist
        if (!CurrencyRegistry.IsSupported(normalizedCode))
        {
            throw new ArgumentException(
                $"Currency '{normalizedCode}' is not supported. Available currencies: {CurrencyRegistry.GetSupportedCodesString()}",
                nameof(currencyCode));
        }

        Amount = Math.Round(amount, 2);
        CurrencyCode = normalizedCode;
    }

    /// <summary>
    /// Creates a Money instance from persistence layer without business rule validation.
    /// This allows loading historical data with deprecated currencies.
    /// For internal use only - EF Core and deserialization scenarios.
    /// </summary>
    internal static Money FromPersistence(decimal amount, string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            throw new ArgumentException("Currency code cannot be null or empty.", nameof(currencyCode));
        }

        if (currencyCode.Length != 3)
        {
            throw new ArgumentException("Currency code must be 3 characters (ISO 4217 format).", nameof(currencyCode));
        }

        if (!IsValidIsoCurrencyFormat(currencyCode))
        {
            throw new ArgumentException(
                "Currency code must contain only letters (ISO 4217 format).",
                nameof(currencyCode));
        }

        // NOTE: Intentionally skips CurrencyRegistry validation for historical data
        return new Money
        {
            Amount = Math.Round(amount, 2),
            CurrencyCode = currencyCode.ToUpperInvariant()
        };
    }

    public static Money operator +(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "add");
        return new Money(first.Amount + second.Amount, first.CurrencyCode);
    }

    public static Money operator -(Money first, Money second)
    {
        ValidateSameCurrency(first, second, "subtract");
        return new Money(first.Amount - second.Amount, first.CurrencyCode);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.CurrencyCode);
    }

    public static Money operator *(decimal multiplier, Money money)
    {
        return new Money(money.Amount * multiplier, money.CurrencyCode);
    }

    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
        {
            throw new DivideByZeroException("Cannot divide money by zero.");
        }

        return new Money(money.Amount / divisor, money.CurrencyCode);
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

    public static Money Zero(string currencyCode) => new Money(0, currencyCode);

    public bool IsZero() => Amount == 0;

    public bool IsPositive() => Amount > 0;

    public bool IsNegative() => Amount < 0;

    public Money Abs() => new Money(Math.Abs(Amount), CurrencyCode);

    public Money Negate() => new Money(-Amount, CurrencyCode);

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
            result[i] = new Money(share, CurrencyCode);
            remainder -= share;
        }

        // Add any remainder to the first allocation to handle rounding
        if (remainder != 0 && ratios.Length > 0)
        {
            result[0] = new Money(result[0].Amount + remainder, CurrencyCode);
        }

        return result;
    }

    /// <summary>
    /// Converts money to a different currency (requires exchange rate)
    /// </summary>
    public Money ConvertTo(string targetCurrencyCode, decimal exchangeRate)
    {
        if (exchangeRate <= 0)
        {
            throw new ArgumentException("Exchange rate must be positive.", nameof(exchangeRate));
        }

        if (CurrencyCode == targetCurrencyCode)
        {
            return this;
        }

        return new Money(Amount * exchangeRate, targetCurrencyCode);
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
        if (first.CurrencyCode != second.CurrencyCode)
        {
            throw new InvalidOperationException(
                $"Cannot {operation} money with different currencies: {first.CurrencyCode} and {second.CurrencyCode}");
        }
    }

    private static bool IsValidIsoCurrencyFormat(string currencyCode)
    {
        // ISO 4217 currency codes consist of 3 uppercase letters
        return currencyCode.All(char.IsLetter);
    }

    public override string ToString() => $"{Amount:N2} {CurrencyCode}";
}