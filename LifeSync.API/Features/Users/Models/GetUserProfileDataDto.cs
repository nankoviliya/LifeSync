using LifeSync.API.Shared;

namespace LifeSync.API.Features.Users.Models;
public record GetUserProfileDataDto
{
    public string UserId { get; init; } = default!;

    public string? UserName { get; init; }

    public string? Email { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public decimal BalanceAmount { get; init; } = default!;

    public string BalanceCurrency { get; init; } = default!;
}