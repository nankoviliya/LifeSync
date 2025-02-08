namespace LifeSync.API.Features.Authentication.Models;

public class RegisterRequest
{
    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;

    public decimal Balance { get; set; }

    public string Currency { get; set; } = default!;

    public Guid LanguageId { get; set; }
}