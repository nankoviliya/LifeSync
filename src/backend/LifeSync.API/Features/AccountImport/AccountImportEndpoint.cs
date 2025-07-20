using FastEndpoints;
using LifeSync.API.Extensions;
using LifeSync.API.Shared.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.AccountImport;

public enum AccountImportFileFormat
{
    Json = 1,
}

public record AccountImportRequest
{
    public AccountImportFileFormat Format { get; init; } = default!;

    public IFormFile File { get; init; } = default!;
}

public sealed class AccountImportEndpoint : Endpoint<AccountImportRequest, MessageResult>
{
    private readonly IAccountImportService _accountImportService;

    public AccountImportEndpoint(IAccountImportService accountImportService)
    {
        _accountImportService = accountImportService;
    }

    public override void Configure()
    {
        Post("api/accountImport");
        AllowFormData();

        Summary(s =>
        {
            s.Summary = "Imports account data from a desired format";
            s.Description = "Parses the account data from the file. Returns success result if data is imported.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(AccountImportRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();
        
        var result = await _accountImportService.ImportAccountDataAsync(userId, request, ct);

        if (!result.IsSuccess)
            await SendAsync(result, 400, cancellation: ct);
        else
            await SendOkAsync(result, cancellation: ct);
    }
}
