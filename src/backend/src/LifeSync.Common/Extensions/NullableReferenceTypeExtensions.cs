using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Extensions;

public static class NullableReferenceTypeExtensions
{
    public static bool HasValue<T>([NotNullWhen(true)] this T? nullable)
        where T : class =>
        nullable is not null;

    public static bool HasNoValue<T>([NotNullWhen(false)] this T? nullable)
        where T : class =>
        !nullable.HasValue();

    public static void Match<T>(this T? nullable, Action<T> hasValueAction, Action hasNoValueAction)
        where T : class
    {
        if (nullable.HasValue())
        {
            hasValueAction(nullable);
        }
        else
        {
            hasNoValueAction();
        }
    }

    public static void MatchElementFound<T>(this T? nullable, Action<T> hasValueAction,
        [CallerArgumentExpression(nameof(nullable))]
        string valueParameterName = null!)
        where T : class
    {
        Guard.ElementFound(nullable, valueParameterName);

        hasValueAction(nullable!);
    }

    public static TResult MatchElementFound<T, TResult>(this T? nullable, Func<T, TResult> hasValueFunc,
        [CallerArgumentExpression(nameof(nullable))]
        string valueParameterName = null!)
        where T : class
    {
        Guard.ElementFound(nullable, valueParameterName);

        return hasValueFunc(nullable!);
    }

    public static T MatchElementFound<T>(this T? nullable,
        [CallerArgumentExpression(nameof(nullable))]
        string valueParameterName = null!)
        where T : class
    {
        Guard.ElementFound(nullable, valueParameterName);

        return nullable!;
    }

    public static TResult Match<T, TResult>(this T? nullable, Func<T, TResult> hasValueFunc,
        Func<TResult> hasNoValueFunc)
        where T : class =>
        nullable.HasValue()
            ? hasValueFunc(nullable)
            : hasNoValueFunc();

    public static async Task<TResult> MatchAsync<T, TResult>(this T? nullable, Func<T, Task<TResult>> hasValueFunc,
        Func<Task<TResult>> hasNoValueFunc)
        where T : class =>
        nullable.HasValue()
            ? await hasValueFunc(nullable)
            : await hasNoValueFunc();

    public static async Task MatchAsync<T>(this T? nullable, Func<T, Task> hasValueFunc,
        Func<Task> hasNoValueFunc)
        where T : class
    {
        if (nullable.HasValue())
        {
            await hasValueFunc(nullable);
        }
        else
        {
            await hasNoValueFunc();
        }
    }

    public static void Execute<T>(this T? nullable, Action<T> executeFunc)
        where T : class
    {
        if (nullable.HasValue())
        {
            executeFunc(nullable);
        }
    }

    public static async Task ExecuteAsync<T>(this T? nullable, Func<T, Task> executeFunc)
        where T : class
    {
        if (nullable.HasValue())
        {
            await executeFunc(nullable);
        }
    }
}