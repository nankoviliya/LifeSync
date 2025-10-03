using System.Runtime.CompilerServices;

namespace LifeSync.Common.Required;

public static class RequiredExtensions
{
    public static RequiredStruct<T> ToRequiredStruct<T>(this T? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) where T : struct =>
        new(value, valueParameterName);

    public static IEnumerable<RequiredStruct<T>> ToRequiredStruct<T>(this IEnumerable<T?> values,
        [CallerArgumentExpression(nameof(values))]
        string valueParameterName = null!)
        where T : struct =>
        values.Select(value => value.ToRequiredStruct(valueParameterName));

    public static RequiredStruct<T> ToRequiredStruct<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) where T : struct =>
        new(value, valueParameterName);

    public static IEnumerable<RequiredStruct<T>> ToRequiredStruct<T>(this IEnumerable<T> values,
        [CallerArgumentExpression(nameof(values))]
        string valueParameterName = null!)
        where T : struct
        => values.Select(value => value.ToRequiredStruct(valueParameterName));

    public static RequiredString ToRequiredString(this string? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) =>
        new(value, valueParameterName);

    public static IEnumerable<RequiredString> ToRequiredStrings(this IEnumerable<string?> values,
        [CallerArgumentExpression(nameof(values))]
        string valueParameterName = null!) =>
        values.Select(value => value.ToRequiredString(valueParameterName));

    public static IEnumerable<T> Values<T>(this IEnumerable<RequiredStruct<T>> values)
        where T : struct =>
        values.Select(x => x.Value);

    public static IEnumerable<string> Values(this IEnumerable<RequiredString> values) => values.Select(x => x.Value);

    public static RequiredReference<T> ToRequiredReference<T>(this T? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) where T : class =>
        new(value, valueParameterName);

    public static IEnumerable<RequiredReference<T>> ToRequiredReferences<T>(this IEnumerable<T?> values,
        [CallerArgumentExpression(nameof(values))]
        string valueParameterName = null!)
        where T : class =>
        values.Select(value => new RequiredReference<T>(value, valueParameterName));

    public static IEnumerable<T> Values<T>(this IEnumerable<RequiredReference<T>> values)
        where T : class =>
        values.Select(x => x.Value);
}