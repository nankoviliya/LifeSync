using FastEndpoints;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Features.Authentication.Register.Services;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Authentication.Register;

public sealed class RegisterEndpoint : Endpoint<RegisterRequest, string>
{
    private readonly IRegisterService _registerService;

    public RegisterEndpoint(IRegisterService registerService) =>
        _registerService = registerService;

    public override void Configure()
    {
        Post("api/auth/register");
        AllowAnonymous();

        Options(x => x.RequireRateLimiting("PublicApi"));

        Summary(s =>
        {
            s.Summary = "Registers a new user";
            s.Description = "Creates a new user account with the provided registration details.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[429] = "Too Many Requests";
        });
    }

    public override async Task HandleAsync(RegisterRequest request, CancellationToken ct)
    {
        MessageResult result = await _registerService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(400, ct);
        }
        else
        {
            await SendOkAsync(result.Message, ct);
        }
    }
}
