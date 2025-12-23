using FastEndpoints;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Refresh.Services;

namespace LifeSync.API.Features.Authentication.Logout;

public sealed class LogoutEndpoint : EndpointWithoutRequest
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public LogoutEndpoint(
        IRefreshTokenService refreshTokenService,
        JwtTokenGenerator jwtTokenGenerator)
    {
        _refreshTokenService = refreshTokenService;
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
            string tokenHash = _jwtTokenGenerator.HashRefreshToken(refreshToken);
            await _refreshTokenService.RevokeRefreshTokenAsync(tokenHash);
        }

        CookieHelper.ClearAuthCookies(HttpContext.Response);

        await Send.NoContentAsync(ct);
    }
}
