namespace LifeSync.API.Features.Authentication.Helpers;

public static class CookieHelper
{
    private const string AccessTokenCookieName = "access_token";
    private const string RefreshTokenCookieName = "refresh_token";

    public static void SetAccessTokenCookie(HttpResponse response, string token)
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
            MaxAge = TimeSpan.FromMinutes(15),
            Path = "/"
        };

        response.Cookies.Append(AccessTokenCookieName, token, options);
    }

    public static void SetRefreshTokenCookie(HttpResponse response, string token)
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
            MaxAge = TimeSpan.FromDays(7),
            Path = "/"
        };

        response.Cookies.Append(RefreshTokenCookieName, token, options);
    }

    public static void ClearAuthCookies(HttpResponse response)
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

        response.Cookies.Delete(AccessTokenCookieName, options);
        response.Cookies.Delete(RefreshTokenCookieName, options);
    }

    public static string? GetRefreshTokenFromCookie(HttpRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return request.Cookies[RefreshTokenCookieName];
    }

    public static string? GetAccessTokenFromCookie(HttpRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return request.Cookies[AccessTokenCookieName];
    }
}
