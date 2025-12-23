using FastEndpoints;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Login.Services;
using LifeSync.API.Features.Authentication.Refresh.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Features.Authentication.Login;

public sealed class LoginEndpoint : Endpoint<LoginRequest>
{
    private readonly ILoginService _loginService;
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ICsrfTokenGenerator _csrfTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginEndpoint(
        ILoginService loginService,
        UserManager<User> userManager,
        JwtTokenGenerator jwtTokenGenerator,
        ICsrfTokenGenerator csrfTokenGenerator,
        IRefreshTokenService refreshTokenService)
    {
        _loginService = loginService;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _csrfTokenGenerator = csrfTokenGenerator;
        _refreshTokenService = refreshTokenService;
    }

    public override void Configure()
    {
        Post("api/auth/login");
        AllowAnonymous();

        Options(x => x.RequireRateLimiting("AuthEndpoints"));

        Summary(s =>
        {
            s.Summary = "Authenticates a user and sets authentication cookies";
            s.Description = "Pass the user credentials to obtain authentication cookies if authentication is successful.";
            s.Responses[200] = "Success";
            s.Responses[401] = "Unauthorized";
            s.Responses[429] = "Too Many Requests";
        });
    }

    public override async Task HandleAsync(LoginRequest request, CancellationToken ct)
    {
        DataResult<TokenResponse> result = await _loginService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await Send.ErrorsAsync(401, ct);
            return;
        }

        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            AddError("User not found.");
            await Send.ErrorsAsync(401, ct);
            return;
        }

        string accessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user);
        string refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        string csrfToken = _csrfTokenGenerator.Generate();

        string deviceInfo = $"{HttpContext.Request.Headers.UserAgent}|{HttpContext.Connection.RemoteIpAddress}";
        string tokenHash = _jwtTokenGenerator.HashRefreshToken(refreshToken);

        await _refreshTokenService.CreateRefreshTokenAsync(user.Id, tokenHash, deviceInfo);

        CookieHelper.SetAccessTokenCookie(HttpContext.Response, accessToken);
        CookieHelper.SetRefreshTokenCookie(HttpContext.Response, refreshToken);
        CookieHelper.SetCsrfTokenCookie(HttpContext.Response, csrfToken);

        await Send.OkAsync(new { message = "Login successful" }, ct);
    }
}