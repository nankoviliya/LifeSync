namespace LifeSync.API.Shared;

public record Currency
{
    internal static readonly Currency None = new Currency("");
    public static readonly Currency Usd = new Currency("USD");
    public static readonly Currency Eur = new Currency("EUR");
    public static readonly Currency Bgn = new Currency("BGN");

    public static readonly IReadOnlyCollection<Currency> All = new[]
    {
        Usd, Eur, Bgn
    };

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(x => x.Code.Equals(code)) ??
               throw new ApplicationException("The currency code is invalid.");
    }

    private Currency(string code)
    {
        Code = code;
    }
    public string Code { get; init; }
}