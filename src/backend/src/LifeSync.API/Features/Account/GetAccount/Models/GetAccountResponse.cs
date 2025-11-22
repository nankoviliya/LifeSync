namespace LifeSync.API.Features.Account.GetAccount.Models;

public record GetAccountResponse
{
    public string UserId { get; init; } = default!;

    public string? UserName { get; init; }

    public string? Email { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public decimal BalanceAmount { get; init; } = default!;

    public string BalanceCurrency { get; init; } = default!;

    public LanguageResponse Language { get; init; } = default!;
}

public record LanguageResponse(Guid Id, string Name, string Code);