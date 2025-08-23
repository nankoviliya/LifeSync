namespace LifeSync.API.Shared;

public record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }

    // Parameterless constructor for EF Core
    private Money() { }

    // Constructor for application use
    public Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money operator -(Money first, Money second)
    {
        if (first.Currency != second.Currency)
        {
            throw new InvalidOperationException("Currencies have to be equal");
        }

        return new Money(first.Amount - second.Amount, first.Currency);
    }

    public static Money Zero() => new Money(0, Currency.None);

    public static Money Zero(Currency currency) => new Money(0, currency);

    public bool IsZero(Currency currency) => this == Zero(currency);
}