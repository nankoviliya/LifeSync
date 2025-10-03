using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Extensions;

public static class NullableValueTypeExtensions
{
    public static bool HasValue<T>([NotNullWhen(true)] this T? nullable)
        where T : struct =>
        nullable.HasValue;

    public static bool HasNoValue<T>([NotNullWhen(false)] this T? nullable)
        where T : struct =>
        !nullable.HasValue();

    public static bool HasNullOrEmpty<T>([NotNullWhen(false)] this T? nullable)
        where T : struct =>
        nullable.HasNoValue() || default(T).Equals(nullable);

    public static T MatchElementFound<T>(this T? nullable,
        [CallerArgumentExpression(nameof(nullable))]
        string valueParameterName = null!)
        where T : struct
    {
        Guard.ElementFound(nullable, valueParameterName);

        return nullable!.Value;
    }
}