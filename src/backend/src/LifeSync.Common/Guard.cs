using LifeSync.Common.Exceptions;
using LifeSync.Common.Extensions;
using System.Runtime.CompilerServices;

namespace LifeSync.Common;

public static class Guard
{
    public static void ArgumentNotNullOrEmptyGuid(Guid? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) =>
        (value is null || value == Guid.Empty)
        .Then(() => throw new ArgumentException($"{valueParameterName} is null or empty"));

    public static void ArgumentNotNullOrEmpty<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!) =>
        (value is null || EqualityComparer<T>.Default.Equals(value, default))
        .Then(() => throw new ArgumentException($"{valueParameterName} is null or empty"));

    public static void ArgumentNotNullOrEmpty(string? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!)
        => string.IsNullOrEmpty(value)
            .Then(() => throw new ArgumentException($"{valueParameterName} is null or empty"));

    public static void ElementFound<T>(T? value,
        [CallerArgumentExpression(nameof(value))]
        string valueParameterName = null!)
        => (value is null).Then(() => throw new ElementNotFoundException($"{valueParameterName} not found"));
}