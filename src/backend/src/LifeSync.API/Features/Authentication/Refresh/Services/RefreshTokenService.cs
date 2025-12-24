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
        string? refreshToken = CookieHelper.GetRefreshTokenFromCookie(request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            CookieHelper.ClearAuthCookies(response);
            return Failure<RefreshResponse>("Refresh token not found.");
        }

        string tokenHash = JwtTokenGenerator.HashRefreshToken(refreshToken);

        RefreshToken? storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);

        if (storedToken is null)
        {
            CookieHelper.ClearAuthCookies(response);
            return Failure<RefreshResponse>("Invalid refresh token.");
        }

        if (!storedToken.IsValid())
        {
            CookieHelper.ClearAuthCookies(response);
            _context.RefreshTokens.Remove(storedToken);
            await _context.SaveChangesAsync();
            return Failure<RefreshResponse>("Refresh token expired or revoked.");
        }

        User user = storedToken.User;

        // Generate new tokens with same device type
        TokenResponse newAccessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, storedToken.DeviceType);
        TokenResponse newRefreshToken = JwtTokenGenerator.GenerateRefreshToken(storedToken.DeviceType);

        string newTokenHash = JwtTokenGenerator.HashRefreshToken(newRefreshToken.Token);

        // Refresh token rotation: delete old, create new
        _context.RefreshTokens.Remove(storedToken);

        RefreshToken newRefreshTokenEntity = RefreshToken.Create(
            user.Id,
            newTokenHash,
            newRefreshToken.Expiry,
            storedToken.DeviceType);

        await _context.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        // Set new cookies
        CookieHelper.SetAccessTokenCookie(response, newAccessToken.Token);
        CookieHelper.SetRefreshTokenCookie(response, newRefreshToken.Token);

        RefreshResponse refreshResponse = new()
        {
            AccessToken = newAccessToken.Token,
            AccessTokenExpiry = newAccessToken.Expiry,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiry = newRefreshToken.Expiry,
            Message = "Token refreshed successfully"
        };

        return Success(refreshResponse);
    }
}