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
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));
        }

        return new RefreshToken
        {
            UserId = userId.Trim(),
            TokenHash = tokenHash.Trim(),
            ExpiresAt = expiresAt,
            DeviceType = deviceType,
            IsRevoked = false,
            RevokedAt = null
        };
    }

    public string UserId { get; init; } = default!;
    public string TokenHash { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
    public DeviceType DeviceType { get; init; }

    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public User User { get; init; } = default!;

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid() => !IsRevoked && !IsExpired();

    public void Revoke()
    {
        if (IsRevoked)
        {
            return;
        }

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}