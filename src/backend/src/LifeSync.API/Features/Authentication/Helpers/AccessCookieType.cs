namespace LifeSync.API.Features.Authentication.Helpers;

public enum AccessCookieType
{
    AccessToken,
    RefreshToken
}

public static class AccessCookieTypeExtensions
{
    public static string ToStringValue(this AccessCookieType type) => type switch
    {
        AccessCookieType.AccessToken => "access_token",
        AccessCookieType.RefreshToken => "refresh_token",
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}