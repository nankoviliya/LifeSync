using FastEndpoints;

namespace LifeSync.API.Features.Authentication.Status;

public sealed class StatusEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("api/auth/status");

        Summary(s =>
        {
            s.Summary = "Checks authentication status";
            s.Description = "Returns 200 if user is authenticated, 401 otherwise.";
            s.Responses[200] = "Authenticated";
            s.Responses[401] = "Not authenticated";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(new { authenticated = true }, ct);
    }
}
