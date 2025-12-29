using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.Common.Required;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LifeSync.API.Features.Authentication.Helpers;

public class JwtTokenGenerator
{
    private readonly ISecretsManager _secretsManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenGenerator(
        ISecretsManager secretsManager,
        JwtSecurityTokenHandler tokenHandler)
    {
        _secretsManager = secretsManager;
        _tokenHandler = tokenHandler;
    }

    public async Task<TokenResponse> GenerateAccessTokenAsync(User user, DeviceType deviceType)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        JwtSecrets jwtSecrets = await _secretsManager.GetJwtSecretsAsync()
                                ?? throw new InvalidOperationException("JWT secrets not configured.");

        Claim[] claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.ToRequiredString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        byte[] keyBytes = Encoding.UTF8.GetBytes(jwtSecrets.SecretKey);
        SymmetricSecurityKey key = new(keyBytes);
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        TimeSpan lifetime = GetAccessTokenLifetime(deviceType);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(lifetime),
            Issuer = jwtSecrets.Issuer,
            Audience = jwtSecrets.Audience,
            SigningCredentials = signingCredentials
        };

        SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = _tokenHandler.WriteToken(token);

        return new TokenResponse { Token = tokenString, Expiry = token.ValidTo };
    }

    /// <summary>
    /// Gets platform-specific access token expiration duration.
    /// Web: 15 minutes (encourage re-authentication)
    /// Mobile: 60 minutes (better UX for mobile apps)
    /// </summary>
    private static TimeSpan GetAccessTokenLifetime(DeviceType deviceType) =>
        deviceType switch
        {
            DeviceType.Web => TimeSpan.FromMinutes(15),
            DeviceType.Mobile => TimeSpan.FromMinutes(60),
            _ => TimeSpan.FromMinutes(15) // Default to most restrictive
        };

    public static TokenResponse GenerateRefreshToken(DeviceType deviceType)
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        string tokenString = Convert.ToBase64String(randomNumber);

        TimeSpan refreshLifetime = GetRefreshTokenLifetime(deviceType);
        DateTime refreshExpiry = DateTime.UtcNow.Add(refreshLifetime);

        return new TokenResponse { Token = tokenString, Expiry = refreshExpiry };
    }

    internal static string HashRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Gets platform-specific refresh token expiration duration.
    /// Web: 7 days (encourage re-authentication)
    /// Mobile: 30 days (better UX for mobile apps)
    /// </summary>
    private static TimeSpan GetRefreshTokenLifetime(DeviceType deviceType) =>
        deviceType switch
        {
            DeviceType.Web => TimeSpan.FromDays(7),
            DeviceType.Mobile => TimeSpan.FromDays(30),
            _ => TimeSpan.FromDays(7) // Default to most restrictive
        };
}