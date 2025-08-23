namespace LifeSync.API.Features.Account.Models;

public record ModifyUserAccountDataDto
{
    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public string LanguageId { get; init; } = default!;
}
