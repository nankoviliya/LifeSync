using LifeSync.API.Models.Abstractions;
using LifeSync.Common.Required;
using System.Text.RegularExpressions;

namespace LifeSync.API.Models.Currencies;

public class Currency : Entity
{
    private static readonly Regex CodeRegex = new(@"^[A-Z]{3}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private Currency() { }

    public static Currency From(
        RequiredString code,
        RequiredString name,
        RequiredString nativeName,
        RequiredString symbol,
        RequiredStruct<int> numericCode)
    {
        string codeValue = code;
        var normalizedCode = codeValue.ToUpperInvariant().Trim();

        ValidateCode(normalizedCode);
        ValidateName(name);
        ValidateNativeName(nativeName);
        ValidateSymbol(symbol);
        ValidateNumericCode(numericCode);

        string nameValue = name;
        string nativeNameValue = nativeName;
        string symbolValue = symbol;
        int numericCodeValue = numericCode;

        return new Currency(
            normalizedCode,
            nameValue.Trim(),
            nativeNameValue.Trim(),
            symbolValue.Trim(),
            numericCodeValue);
    }

    private Currency(
        string code,
        string name,
        string nativeName,
        string symbol,
        int numericCode)
    {
        Code = code;
        Name = name;
        NativeName = nativeName;
        Symbol = symbol;
        NumericCode = numericCode;
    }

    // ISO 4217 three-letter code (e.g., USD, EUR) – must be unique
    public string Code { get; } = default!;

    public string Name { get; } = default!;

    public string NativeName { get; private set; } = default!;

    public string Symbol { get; private set; } = default!;

    // ISO numeric code (e.g., 840 for USD)
    public int NumericCode { get; private set; }

    /// <summary>
    /// Updates the native name of the currency
    /// </summary>
    public void UpdateNativeName(RequiredString nativeName)
    {
        ValidateNativeName(nativeName);
        string value = nativeName;
        NativeName = value.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the symbol of the currency
    /// </summary>
    public void UpdateSymbol(RequiredString symbol)
    {
        ValidateSymbol(symbol);
        string value = symbol;
        Symbol = value.Trim();
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the currency code matches the given code
    /// </summary>
    public bool IsCode(string code) =>
        Code.Equals(code?.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if this is a valid ISO 4217 currency
    /// </summary>
    public bool IsValid() =>
        IsValidCode(Code) && NumericCode > 0 && NumericCode < 1000;

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Currency code cannot be null or empty.", nameof(code));
        }

        if (!CodeRegex.IsMatch(code))
        {
            throw new ArgumentException(
                "Currency code must be a 3-letter ISO 4217 code (e.g., USD, EUR).",
                nameof(code));
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Currency name cannot be null or empty.", nameof(name));
        }

        if (name.Trim().Length < 2)
        {
            throw new ArgumentException("Currency name must be at least 2 characters.", nameof(name));
        }
    }

    private static void ValidateNativeName(string nativeName)
    {
        if (string.IsNullOrWhiteSpace(nativeName))
        {
            throw new ArgumentException("Native name cannot be null or empty.", nameof(nativeName));
        }
    }

    private static void ValidateSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Currency symbol cannot be null or empty.", nameof(symbol));
        }
    }

    private static void ValidateNumericCode(int numericCode)
    {
        if (numericCode <= 0)
        {
            throw new ArgumentException(
                "Numeric code must be a positive integer.",
                nameof(numericCode));
        }

        if (numericCode >= 1000)
        {
            throw new ArgumentException(
                "Numeric code must be less than 1000 per ISO 4217 standard.",
                nameof(numericCode));
        }
    }

    private static bool IsValidCode(string code) =>
        !string.IsNullOrWhiteSpace(code) && CodeRegex.IsMatch(code);

    public override string ToString() => $"{Name} ({Code})";
}