using LifeSync.API.Models.Abstractions;
using LifeSync.Common.Required;
using System.Text.RegularExpressions;

namespace LifeSync.API.Models.Languages;

public class Language : Entity
{
    // ISO 639-1 (2-letter) or ISO 639-2 (3-letter) language code
    private static readonly Regex CodeRegex = new(@"^[a-z]{2}(-[A-Z]{2})?$|^[a-z]{3}$", RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

    private Language() { }

    public static Language From(
        RequiredString name,
        RequiredString code)
    {
        string codeValue = code;
        string nameValue = name;

        var normalizedCode = NormalizeCode(codeValue);
        var normalizedName = nameValue.Trim();

        ValidateCode(normalizedCode);
        ValidateName(normalizedName);

        return new Language(normalizedName, normalizedCode);
    }

    private Language(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = default!;

    // ISO 639 language code (e.g., en, en-US, eng)
    public string Code { get; private set; } = default!;

    /// <summary>
    /// Updates the language name
    /// </summary>
    public void UpdateName(RequiredString name)
    {
        string nameValue = name;
        var normalizedName = nameValue.Trim();
        ValidateName(normalizedName);
        Name = normalizedName;
        MarkAsUpdated();
    }

    /// <summary>
    /// Updates the language code
    /// </summary>
    public void UpdateCode(RequiredString code)
    {
        string codeValue = code;
        var normalizedCode = NormalizeCode(codeValue);
        ValidateCode(normalizedCode);
        Code = normalizedCode;
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the language code matches the given code
    /// </summary>
    public bool IsCode(string code) =>
        Code.Equals(NormalizeCode(code), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the base language code without region (e.g., "en" from "en-US")
    /// </summary>
    public string GetBaseLanguageCode()
    {
        var dashIndex = Code.IndexOf('-');
        return dashIndex > 0 ? Code[..dashIndex] : Code;
    }

    /// <summary>
    /// Checks if this language has a region specifier (e.g., en-US)
    /// </summary>
    public bool HasRegion() => Code.Contains('-');

    /// <summary>
    /// Gets the region code if present (e.g., "US" from "en-US")
    /// </summary>
    public string? GetRegionCode()
    {
        var dashIndex = Code.IndexOf('-');
        return dashIndex > 0 ? Code[(dashIndex + 1)..] : null;
    }

    private static void ValidateCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Language code cannot be null or empty.", nameof(code));
        }

        if (!CodeRegex.IsMatch(code))
        {
            throw new ArgumentException(
                "Language code must be a valid ISO 639 code (e.g., en, en-US, eng).",
                nameof(code));
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Language name cannot be null or empty.", nameof(name));
        }

        if (name.Length < 2)
        {
            throw new ArgumentException("Language name must be at least 2 characters.", nameof(name));
        }
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return code;
        }

        code = code.Trim();

        // Handle region codes (e.g., en-US)
        if (code.Contains('-'))
        {
            var parts = code.Split('-');
            if (parts.Length == 2)
            {
                return $"{parts[0].ToLowerInvariant()}-{parts[1].ToUpperInvariant()}";
            }
        }

        return code.ToLowerInvariant();
    }

    public override string ToString() => $"{Name} ({Code})";
}