using FastEndpoints;
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
    }

    public override async Task HandleAsync(AccountImportRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _accountImportService.ImportAccountDataAsync(userId, request.File, request.Format, ct);

        if (!result.IsSuccess)
            await SendAsync(result, 400, cancellation: ct);
        else
            await SendOkAsync(result, cancellation: ct);
    }
}
