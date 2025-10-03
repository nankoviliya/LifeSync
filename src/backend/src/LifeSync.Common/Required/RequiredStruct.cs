using LifeSync.Common.Extensions;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Required;

public readonly record struct RequiredStruct<T>
    where T : struct
{
    public RequiredStruct(T? value, [CallerArgumentExpression(nameof(value))] string valueParameterName = null!)
    {
        Value = value.MatchElementFound(valueParameterName);

        Guard.ArgumentNotNullOrEmpty(Value, valueParameterName);
    }

    public T Value { get; }

    public override string ToString() => Value.ToString()!;

    public static implicit operator T(RequiredStruct<T> requiredStruct) => requiredStruct.Value;
}