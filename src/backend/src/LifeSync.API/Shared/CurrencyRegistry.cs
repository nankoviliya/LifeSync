namespace LifeSync.API.Shared;

/// <summary>
/// Registry of supported currencies in the application.
/// To add a new currency, simply add it to this registry - no database changes needed.
/// </summary>
public static class CurrencyRegistry
{
    // Supported currencies
    public static readonly CurrencyInfo USD = new("USD", "US Dollar", "United States Dollar", "$");
    public static readonly CurrencyInfo EUR = new("EUR", "Euro", "Euro", "€");
    public static readonly CurrencyInfo BGN = new("BGN", "Bulgarian Lev", "Български лев", "лв");
    public static readonly CurrencyInfo UAH = new("UAH", "Ukrainian Hryvnia", "Українська гривня", "₴");
    public static readonly CurrencyInfo RUB = new("RUB", "Russian Ruble", "Российский рубль", "₽");

    private static readonly Dictionary<string, CurrencyInfo> _byCode = new(StringComparer.OrdinalIgnoreCase)
    {
        { "USD", USD },
        { "EUR", EUR },
        { "BGN", BGN },
        { "UAH", UAH },
        { "RUB", RUB }
    };

    /// <summary>
    /// Gets all supported currencies
    /// </summary>
    public static IReadOnlyCollection<CurrencyInfo> All => _byCode.Values;

    /// <summary>
    /// Gets currency information by code
    /// </summary>
    /// <param name="code">ISO 4217 currency code (e.g., USD, EUR)</param>
    /// <returns>CurrencyInfo if found, null otherwise</returns>
    public static CurrencyInfo? GetByCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        _byCode.TryGetValue(code, out var currency);
        return currency;
    }

    /// <summary>
    /// Checks if a currency code is supported
    /// </summary>
    /// <param name="code">ISO 4217 currency code (e.g., USD, EUR)</param>
    public static bool IsSupported(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return _byCode.ContainsKey(code);
    }

    /// <summary>
    /// Gets a comma-separated list of all supported currency codes
    /// </summary>
    public static string GetSupportedCodesString() =>
        string.Join(", ", _byCode.Keys);
}

/// <summary>
/// Represents currency information
/// </summary>
/// <param name="Code">ISO 4217 three-letter currency code (e.g., USD, EUR)</param>
/// <param name="Name">English name of the currency (e.g., "US Dollar")</param>
/// <param name="NativeName">Native name of the currency (e.g., "United States Dollar")</param>
/// <param name="Symbol">Currency symbol (e.g., "$", "€")</param>
public record CurrencyInfo(string Code, string Name, string NativeName, string Symbol)
{
    public override string ToString() => $"{Name} ({Code})";
}
