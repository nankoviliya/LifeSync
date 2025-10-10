namespace LifeSync.API.Features.Authentication.Helpers;

public class TokenResponse
{
    public string Token { get; set; } = default!;
    public DateTime Expiry { get; set; }
}