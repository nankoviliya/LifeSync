using FastEndpoints;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.AccountImport;

public enum AccountImportFileFormat
{
    Json = 1
}

public record AccountImportRequest
{
    public AccountImportFileFormat Format { get; init; }

    public IFormFile File { get; init; } = default!;
}

public sealed class AccountImportEndpoint : Endpoint<AccountImportRequest, string>
{
    private readonly IAccountImportService _accountImportService;

    public AccountImportEndpoint(IAccountImportService accountImportService) =>
        _accountImportService = accountImportService;

    public override void Configure()
    {
        Post("api/accountImport");
        AllowFormData();

        Summary(s =>
        {
            s.Summary = "[PREVIEW] Imports account data from a desired format";
            s.Description = "**PREVIEW WARNING**: Validations are not implemented yet" +
                            "Parses the account data from the file. Returns success result if data is imported.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(AccountImportRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        MessageResult result = await _accountImportService.ImportAccountDataAsync(userId, request, ct);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await Send.ErrorsAsync(400, ct);
        }
        else
        {
            await Send.OkAsync(result.Message, ct);
        }
    }
}