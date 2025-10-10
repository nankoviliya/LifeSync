namespace LifeSync.API.Features.Account.UpdateAccount.Models;

public record UpdateAccountRequest
{
    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string LanguageId { get; init; } = default!;
}
