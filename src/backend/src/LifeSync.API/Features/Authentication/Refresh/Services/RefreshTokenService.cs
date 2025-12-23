using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Refresh.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Authentication.Refresh.Services;

public sealed class RefreshTokenService : BaseService, IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenService(
        ApplicationDbContext context,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
    }

    /// <summary>
    /// Validates refresh token from cookie and issues new tokens.
    /// Implements refresh token rotation for security.
    /// </summary>
    public async Task<DataResult<RefreshResponse>> RefreshTokenAsync(HttpRequest request, HttpResponse response)
    {
        // Extract refresh token from cookie
        string? refreshToken = CookieHelper.GetRefreshTokenFromCookie(request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            CookieHelper.ClearAuthCookies(response);
            return Failure<RefreshResponse>("Refresh token not found.");
        }

        // Hash and lookup token
        string tokenHash = _jwtTokenGenerator.HashRefreshToken(refreshToken);

        RefreshToken? storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken is null)
        {
            CookieHelper.ClearAuthCookies(response);
            return Failure<RefreshResponse>("Invalid refresh token.");
        }

        // Validate token
        if (!storedToken.IsValid())
        {
            CookieHelper.ClearAuthCookies(response);
            _context.RefreshTokens.Remove(storedToken); // Hard delete invalid token
            await _context.SaveChangesAsync();
            return Failure<RefreshResponse>("Refresh token expired or revoked.");
        }

        User user = storedToken.User;

        // Generate new tokens with same device type
        TokenResponse newAccessToken = await _jwtTokenGenerator.GenerateJwtTokenAsync(user, storedToken.DeviceType);
        string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        string newTokenHash = _jwtTokenGenerator.HashRefreshToken(newRefreshToken);

        // Calculate expiration (maintain same device type)
        TimeSpan lifetime = _jwtTokenGenerator.GetRefreshTokenLifetime(storedToken.DeviceType);
        DateTime expiresAt = DateTime.UtcNow.Add(lifetime);

        // Refresh token rotation: delete old, create new
        _context.RefreshTokens.Remove(storedToken);

        RefreshToken newRefreshTokenEntity = RefreshToken.Create(
            user.Id,
            newTokenHash,
            expiresAt,
            storedToken.DeviceType);

        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        // Set new cookies
        CookieHelper.SetAccessTokenCookie(response, newAccessToken.Token);
        CookieHelper.SetRefreshTokenCookie(response, newRefreshToken);

        // Build and return response
        RefreshResponse refreshResponse = new()
        {
            AccessToken = newAccessToken.Token,
            AccessTokenExpiry = newAccessToken.Expiry,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiry = expiresAt,
            Message = "Token refreshed successfully"
        };

        return Success(refreshResponse);
    }

    /// <summary>
    /// Revokes all refresh tokens for a specific user.
    /// Used for security operations like password change or "logout all devices".
    /// </summary>
    public async Task RevokeAllUserTokensAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        List<RefreshToken> userTokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
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

    /// <summary>
    /// Hard deletes expired and revoked tokens from database.
    /// Called by background cleanup job.
    /// </summary>
    public async Task CleanupExpiredTokensAsync()
    {
        DateTime cutoffDate = DateTime.UtcNow;

        // Hard delete tokens that are expired OR revoked
        List<RefreshToken> tokensToDelete = await _context.RefreshTokens
            .Where(t => t.ExpiresAt < cutoffDate || t.IsRevoked)
            .ToListAsync();

        if (tokensToDelete.Count > 0)
        {
            _context.RefreshTokens.RemoveRange(tokensToDelete);
            await _context.SaveChangesAsync();
        }
    }
}