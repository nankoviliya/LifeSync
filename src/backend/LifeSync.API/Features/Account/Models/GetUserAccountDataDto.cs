using LifeSync.API.Models.Languages;

namespace LifeSync.API.Features.Account.Models;
public record GetUserAccountDataDto
{
    public string UserId { get; init; } = default!;

    public string? UserName { get; init; }

    public string? Email { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public decimal BalanceAmount { get; init; } = default!;

    public string BalanceCurrency { get; init; } = default!;

    public Language Language { get; init; } = default!;
}