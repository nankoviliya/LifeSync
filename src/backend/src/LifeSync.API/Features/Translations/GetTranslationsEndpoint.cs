using FastEndpoints;
using LifeSync.API.Features.Translations.Models;
using LifeSync.API.Features.Translations.Services.Contracts;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Translations;

public sealed class GetTranslationsEndpoint : Endpoint<GetTranslationsRequest, IReadOnlyDictionary<string, string>>
{
    private readonly ITranslationsService _translationsService;

    public GetTranslationsEndpoint(ITranslationsService translationsService) =>
        _translationsService = translationsService;

    public override void Configure()
    {
        Get("api/translations");
        AllowAnonymous();

        Options(x => x.RequireRateLimiting("PublicApi"));

        Summary(s =>
        {
            s.Summary = "Retrieves translations for specified language code";
            s.Description = "Retrieves translations inside dictionary based on the provided language code.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[429] = "Too Many Requests";
        });
    }

    public override async Task HandleAsync(GetTranslationsRequest request, CancellationToken ct)
    {
        DataResult<IReadOnlyDictionary<string, string>> result =
            await _translationsService.GetTranslationsByLanguageCodeAsync(request.LanguageCode, ct);

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
