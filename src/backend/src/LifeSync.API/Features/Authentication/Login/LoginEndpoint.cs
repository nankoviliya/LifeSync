using FastEndpoints;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Login.Services;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Login;

public sealed class LoginEndpoint : Endpoint<LoginRequest, TokenResponse>
{
    private readonly ILoginService _loginService;

    public LoginEndpoint(ILoginService loginService) =>
        _loginService = loginService;

    public override void Configure()
    {
        Post("api/auth/login");
        AllowAnonymous();

        Options(x => x.RequireRateLimiting("PublicApi"));

        Summary(s =>
        {
            s.Summary = "Authenticates a user and returns a JWT token";
            s.Description = "Pass the user credentials to obtain a valid JWT token if authentication is successful.";
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
        }
        else
        {
            await Send.OkAsync(result.Data, ct);
        }
    }
}