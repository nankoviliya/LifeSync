using LifeSync.API.Models.RefreshTokens;

namespace LifeSync.API.Features.Authentication.Refresh.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshTokenAsync(string userId, string tokenHash, string deviceInfo);
    Task<RefreshToken?> ValidateRefreshTokenAsync(string tokenHash);
    Task RevokeRefreshTokenAsync(string tokenHash);
    Task RevokeAllUserTokensAsync(string userId);
    Task CleanupExpiredTokensAsync();
}
