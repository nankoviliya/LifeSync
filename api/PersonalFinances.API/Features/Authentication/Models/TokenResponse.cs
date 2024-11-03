namespace PersonalFinances.API.Features.Authentication.Models;

public class TokenResponse
{
    public string Token { get; set; } = default!;
    public DateTime Expiry { get; set; }
}