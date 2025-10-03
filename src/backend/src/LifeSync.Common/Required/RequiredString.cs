using LifeSync.Common.Extensions;
using System.Runtime.CompilerServices;

namespace LifeSync.Common.Required;

public readonly record struct RequiredString
{
    public RequiredString(string? value, [CallerArgumentExpression(nameof(value))] string valueParameterName = null!) =>
        Value = value
            .MatchNonEmptyElementFound(valueParameterName)
            .Trim()
            .MatchNonEmptyElementFound(valueParameterName);

    public string Value { get; }

    public override string ToString() => Value;

    public static implicit operator string(RequiredString required) => required.Value;
}