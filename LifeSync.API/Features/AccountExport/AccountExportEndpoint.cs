﻿using FastEndpoints;
using LifeSync.API.Shared.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.AccountExport;

public enum ExportAccountFileFormat
{
    Json = 1,
}

public record ExportAccountRequest
{
    public ExportAccountFileFormat Format { get; init; } = default!;
}

public record ExportAccountResponse
{
    public byte[] Data { get; set; } = [];

    public string ContentType { get; set; } = default!;

    public string FileName { get; set; } = default!;
}

public sealed class AccountExportEndpoint : Endpoint<ExportAccountRequest, DataResult<ExportAccountResponse>>
{
    private readonly IAccountExportService _accountExportService;

    public AccountExportEndpoint(IAccountExportService accountExportService)
    {
        _accountExportService = accountExportService;
    }

    public override void Configure()
    {
        Post("api/accountExport");
        AllowFormData();

        Summary(s =>
        {
            s.Summary = "Exports account data into a desired file format";
            s.Description = "Gets the profile information of the currently authenticated user. Returns data in specified file format.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(ExportAccountRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var result = await _accountExportService.ExportAccountData(userId, request, ct);

        if (!result.IsSuccess)
            await SendAsync(result, 400, cancellation: ct);
        else
            await SendOkAsync(result, cancellation: ct);
    }
}
