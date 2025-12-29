namespace LifeSync.API.Features.Authentication.Login.Models;

public sealed record LoginResponse
{
    public required string AccessToken { get; init; }
    public required DateTime AccessTokenExpiry { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiry { get; init; }
    public required string Message { get; init; }
}
