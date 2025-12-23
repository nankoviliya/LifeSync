using FastEndpoints;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Refresh.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.RefreshTokens;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Features.Authentication.Refresh;

public sealed class RefreshEndpoint : EndpointWithoutRequest
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ICsrfTokenGenerator _csrfTokenGenerator;

    public RefreshEndpoint(
        IRefreshTokenService refreshTokenService,
        UserManager<User> userManager,
        JwtTokenGenerator jwtTokenGenerator,
        ICsrfTokenGenerator csrfTokenGenerator)
    {
        _refreshTokenService = refreshTokenService;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _csrfTokenGenerator = csrfTokenGenerator;
    }

    public override void Configure()
    {
        Post("api/auth/refresh");
        AllowAnonymous();

        Options(x => x.RequireRateLimiting("AuthEndpoints"));

        Summary(s =>
        {
            s.Summary = "Refreshes access token using refresh token";
            s.Description = "Uses the refresh token cookie to generate new access and refresh tokens.";
            s.Responses[200] = "Success - New tokens set in cookies";
            s.Responses[401] = "Unauthorized - Invalid or expired refresh token";
            s.Responses[429] = "Too Many Requests";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        string? refreshToken = CookieHelper.GetRefreshTokenFromCookie(HttpContext.Request);

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            CookieHelper.ClearAuthCookies(HttpContext.Response);
            await Send.ErrorsAsync(401, ct);
            return;
        }

        string tokenHash = _jwtTokenGenerator.HashRefreshToken(refreshToken);

        RefreshToken? storedToken = await _refreshTokenService.ValidateRefreshTokenAsync(tokenHash);

        if (storedToken is null)
        {
            CookieHelper.ClearAuthCookies(HttpContext.Response);
            await Send.ErrorsAsync(401, ct);
            return;
        }

        User? user = await _userManager.FindByIdAsync(storedToken.UserId);

        if (user is null)
        {
            CookieHelper.ClearAuthCookies(HttpContext.Response);
            await _refreshTokenService.RevokeRefreshTokenAsync(tokenHash);
            await Send.ErrorsAsync(401, ct);
            return;
        }

        await _refreshTokenService.RevokeRefreshTokenAsync(tokenHash);

        string newAccessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user);
        string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        string newCsrfToken = _csrfTokenGenerator.Generate();

        string deviceInfo = $"{HttpContext.Request.Headers.UserAgent}|{HttpContext.Connection.RemoteIpAddress}";
        string newTokenHash = _jwtTokenGenerator.HashRefreshToken(newRefreshToken);

        await _refreshTokenService.CreateRefreshTokenAsync(user.Id, newTokenHash, deviceInfo);

        CookieHelper.SetAccessTokenCookie(HttpContext.Response, newAccessToken);
        CookieHelper.SetRefreshTokenCookie(HttpContext.Response, newRefreshToken);
        CookieHelper.SetCsrfTokenCookie(HttpContext.Response, newCsrfToken);

        await Send.OkAsync(new { message = "Token refreshed successfully" }, ct);
    }
}
