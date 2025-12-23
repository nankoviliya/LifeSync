using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Authentication.Refresh.Services;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenService(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(string userId, string tokenHash, string deviceInfo)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash cannot be null or empty.", nameof(tokenHash));
        }

        if (string.IsNullOrWhiteSpace(deviceInfo))
        {
            throw new ArgumentException("Device info cannot be null or empty.", nameof(deviceInfo));
        }

        DateTime expiresAt = DateTime.UtcNow.AddDays(7);

        RefreshToken refreshToken = RefreshToken.Create(userId, tokenHash, expiresAt, deviceInfo);

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> ValidateRefreshTokenAsync(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return null;
        }

        RefreshToken? token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsDeleted);

        if (token is null)
        {
            return null;
        }

        if (!token.IsValid())
        {
            return null;
        }

        return token;
    }

    public async Task RevokeRefreshTokenAsync(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            return;
        }

        RefreshToken? token = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && !t.IsDeleted);

        if (token is null)
        {
            return;
        }

        if (!token.IsRevoked)
        {
            token.Revoke();
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllUserTokensAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        List<RefreshToken> userTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked && !t.IsDeleted)
            .ToListAsync();

        foreach (RefreshToken token in userTokens)
        {
            token.Revoke();
        }

        if (userTokens.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task CleanupExpiredTokensAsync()
    {
        DateTime cutoffDate = DateTime.UtcNow.AddDays(-30);

        List<RefreshToken> expiredTokens = await _context.RefreshTokens
            .Where(t => (t.IsRevoked || t.ExpiresAt < DateTime.UtcNow) && t.CreatedAt < cutoffDate && !t.IsDeleted)
            .ToListAsync();

        foreach (RefreshToken token in expiredTokens)
        {
            token.MarkAsDeleted();
        }

        if (expiredTokens.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }
}
