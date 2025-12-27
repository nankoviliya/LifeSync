namespace LifeSync.API.Features.Authentication.Refresh.Models;

public sealed record RefreshResponse
{
    public required string AccessToken { get; init; }
    public required DateTime AccessTokenExpiry { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime RefreshTokenExpiry { get; init; }
    public required string Message { get; init; }
}
