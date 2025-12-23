using System.Security.Cryptography;
using System.Text;

namespace LifeSync.API.Middleware;

public sealed class CsrfProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "GET", "HEAD", "OPTIONS", "TRACE"
    };

    private const string CsrfCookieName = "csrf_token";
    private const string CsrfHeaderName = "X-CSRF-TOKEN";

    public CsrfProtectionMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (ShouldValidateCsrf(context))
        {
            string? cookieToken = context.Request.Cookies[CsrfCookieName];
            string? headerToken = context.Request.Headers[CsrfHeaderName].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(cookieToken) || string.IsNullOrWhiteSpace(headerToken))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("CSRF token validation failed.");
                return;
            }

            if (!AreTokensEqual(cookieToken, headerToken))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("CSRF token validation failed.");
                return;
            }
        }

        await _next(context);
    }

    private static bool ShouldValidateCsrf(HttpContext context)
    {
        if (SafeMethods.Contains(context.Request.Method))
        {
            return false;
        }

        Endpoint? endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            return false;
        }

        bool isAnonymous = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() is not null;
        if (isAnonymous)
        {
            return false;
        }

        return true;
    }

    private static bool AreTokensEqual(string token1, string token2)
    {
        byte[] bytes1 = Encoding.UTF8.GetBytes(token1);
        byte[] bytes2 = Encoding.UTF8.GetBytes(token2);

        if (bytes1.Length != bytes2.Length)
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(bytes1, bytes2);
    }
}
