using FastEndpoints;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LifeSync.API.Features.Authentication.Logout;

public sealed class LogoutEndpoint : EndpointWithoutRequest
{
    private readonly ApplicationDbContext _context;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public LogoutEndpoint(
        ApplicationDbContext context,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public override void Configure()
    {
        Post("api/auth/logout");

        Summary(s =>
        {
            s.Summary = "Logs out the current user";
            s.Description = "Revokes the refresh token and clears authentication cookies.";
            s.Responses[204] = "Success - User logged out";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? refreshToken = CookieHelper.GetRefreshTokenFromCookie(HttpContext.Request);

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            string tokenHash = JwtTokenGenerator.HashRefreshToken(refreshToken);

            RefreshToken? token = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

            if (token is not null)
            {
                _context.RefreshTokens.Remove(token);
                await _context.SaveChangesAsync(ct);
            }
        }

        CookieHelper.ClearAuthCookies(HttpContext.Response);

        await Send.NoContentAsync(ct);
    }
}