namespace LifeSync.Common.Extensions;

public static class StringExtensions
{
    public static string ToRequiredString(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
        }

        return input;
    }

    public static Guid ToRequiredGuid(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
        }

        if (!Guid.TryParse(input, out Guid result))
        {
            throw new FormatException($"Unable to parse '{input}' as a valid GUID.");
        }

        return result;
    }

    public static Guid? ToNullableGuid(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        return Guid.TryParse(input, out Guid result) ? result : null;
    }
}