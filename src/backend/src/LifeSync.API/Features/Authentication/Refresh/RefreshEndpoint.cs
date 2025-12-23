using FastEndpoints;
using LifeSync.API.Features.Authentication.Refresh.Models;
using LifeSync.API.Features.Authentication.Refresh.Services;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Refresh;

public sealed class RefreshEndpoint : EndpointWithoutRequest
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshEndpoint(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
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
        DataResult<RefreshResponse> result = await _refreshTokenService.RefreshTokenAsync(
            HttpContext.Request,
            HttpContext.Response);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await Send.ErrorsAsync(401, ct);
            return;
        }

        await Send.OkAsync(result.Data, ct);
    }
}
