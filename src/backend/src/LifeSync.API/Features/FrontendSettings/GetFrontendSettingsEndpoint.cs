using FastEndpoints;
using LifeSync.API.Features.FrontendSettings.Models;
using LifeSync.API.Features.FrontendSettings.Services;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.FrontendSettings;

public sealed class GetFrontendSettingsEndpoint : EndpointWithoutRequest<FrontendSettingsResponse>
{
    private readonly IFrontendSettingsService _frontendSettingsService;

    public GetFrontendSettingsEndpoint(IFrontendSettingsService frontendSettingsService) =>
        _frontendSettingsService = frontendSettingsService;

    public override void Configure()
    {
        Get("api/frontendSettings");
        AllowAnonymous();

        Summary(s =>
        {
            s.Summary = "Retrieves frontend settings";
            s.Description = "Gets the current frontend settings configuration, like: language options, currency options etc.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        DataResult<FrontendSettingsResponse> result = await _frontendSettingsService.GetFrontendSettingsAsync(ct);

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
            await SendOkAsync(result.Data, ct);
        }
    }
}
