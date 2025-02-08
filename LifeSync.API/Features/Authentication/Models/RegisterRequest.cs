namespace LifeSync.API.Features.Authentication.Models;

public class RegisterRequest
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required decimal Balance { get; set; }

    public required string Currency { get; set; }

    public required Guid LanguageId { get; set; }
}