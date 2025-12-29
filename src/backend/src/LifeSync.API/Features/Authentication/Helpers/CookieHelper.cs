namespace LifeSync.API.Features.Authentication.Helpers;

public static class CookieHelper
{
    public static string? GetAccessTokenFromCookie(HttpRequest request)
        => GetCookie(request, AccessCookieType.AccessToken);

    public static string? GetRefreshTokenFromCookie(HttpRequest request)
        => GetCookie(request, AccessCookieType.RefreshToken);

    public static void SetAccessTokenCookie(HttpResponse response, string token) =>
        SetCookie(response, AccessCookieType.AccessToken, token, TimeSpan.FromMinutes(1));

    public static void SetRefreshTokenCookie(HttpResponse response, string token) =>
        SetCookie(response, AccessCookieType.RefreshToken, token, TimeSpan.FromDays(7));

    public static void ClearAuthCookies(HttpResponse response)
    {
        DeleteCookie(response, AccessCookieType.AccessToken);
        DeleteCookie(response, AccessCookieType.RefreshToken);
    }

    private static string? GetCookie(HttpRequest request, AccessCookieType cookieType)
    {
        ArgumentNullException.ThrowIfNull(request);
        return request.Cookies[cookieType.ToStringValue()];
    }

    private static void SetCookie(
        HttpResponse response,
        AccessCookieType cookieType,
        string token,
        TimeSpan maxAge)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = maxAge,
            Path = "/"
        };

        response.Cookies.Append(cookieType.ToStringValue(), token, options);
    }

    private static void DeleteCookie(HttpResponse response, AccessCookieType cookieType)
    {
        if (response is null)
        {
            throw new ArgumentNullException(nameof(response));
        }

        CookieOptions options = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromSeconds(-1),
            Path = "/"
        };

        response.Cookies.Delete(cookieType.ToStringValue(), options);
    }
}