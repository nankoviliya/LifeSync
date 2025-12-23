using LifeSync.API.Models.Abstractions;
using LifeSync.API.Models.ApplicationUser;

namespace LifeSync.API.Models.RefreshTokens;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    public static RefreshToken Create(
        string userId,
        string tokenHash,
        DateTime expiresAt,
        DeviceType deviceType)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash cannot be null or empty.", nameof(tokenHash));
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));
        }

        RefreshToken token = new()
        {
            UserId = userId.Trim(),
            TokenHash = tokenHash.Trim(),
            ExpiresAt = expiresAt,
            DeviceType = deviceType,
            IsRevoked = false,
            RevokedAt = null
        };

        return token;
    }

    public string UserId { get; private set; } = default!;

    public string TokenHash { get; private set; } = default!;

    public DateTime ExpiresAt { get; private set; }

    public DeviceType DeviceType { get; private set; }

    public bool IsRevoked { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public User User { get; init; } = default!;

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public bool IsValid() => !IsRevoked && !IsExpired();

    public void Revoke()
    {
        if (IsRevoked)
        {
            throw new InvalidOperationException("Token is already revoked.");
        }

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
