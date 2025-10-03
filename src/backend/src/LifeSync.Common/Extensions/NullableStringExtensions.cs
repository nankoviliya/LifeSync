using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Extensions;

public static class NullableStringExtensions
{
    public static Guid ToGuid(this string? nullable)
        => Guid.Parse(nullable.MatchNonEmptyElementFound());

    public static bool HasNullOrEmptyValue([NotNullWhen(true)] this string? nullable)
        => string.IsNullOrEmpty(nullable);

    public static bool HasNonEmptyValue([NotNullWhen(true)] this string? nullable)
        => !string.IsNullOrEmpty(nullable);

    public static bool HasNoStringValue([NotNullWhen(false)] this string? nullable)
        => !nullable.HasNonEmptyValue();

    public static void MatchNonEmptyValue(this string? nullable, Action<string> hasValueAction, Action hasNoValueAction)
    {
        if (nullable.HasNonEmptyValue())
        {
            hasValueAction(nullable);
        }
        else
        {
            hasNoValueAction();
        }
    }

    public static string MatchNonEmptyElementFound(this string? nullable,
        [CallerArgumentExpression(nameof(nullable))] string valueParameterName = null!)
    {
        Guard.ArgumentNotNullOrEmpty(nullable, valueParameterName);

        return nullable!;
    }
}