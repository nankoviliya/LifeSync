namespace LifeSync.API.Features.Authentication.Register.Models;

public record RegisterRequest
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public required decimal Balance { get; init; }

    public required string Currency { get; init; }

    public required Guid LanguageId { get; init; }
}
