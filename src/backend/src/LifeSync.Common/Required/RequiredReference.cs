using LifeSync.Common.Extensions;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Required;

public readonly record struct RequiredReference<T>
    where T : class
{
    public RequiredReference(T? value, [CallerArgumentExpression(nameof(value))] string valueParameterName = null!) =>
        Value = value.MatchElementFound(valueParameterName);

    public T Value { get; }

    public override string ToString() => Value.ToString()!;

    public static implicit operator T(RequiredReference<T> required) => required.Value;
}