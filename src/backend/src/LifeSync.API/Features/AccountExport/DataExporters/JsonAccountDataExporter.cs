using LifeSync.API.Features.AccountExport.Models;
using System.Text.Json;

namespace LifeSync.API.Features.AccountExport.DataExporters;

public class JsonAccountDataExporter : IAccountDataExporter
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new() { WriteIndented = true };

    public ExportAccountFileFormat Format => ExportAccountFileFormat.Json;

    public async Task<ExportAccountResponse> Export(ExportAccountData accountData, CancellationToken cancellationToken)
    {
        await using MemoryStream memoryStream = new();

        await JsonSerializer.SerializeAsync(memoryStream, accountData, CachedJsonSerializerOptions, cancellationToken);

        memoryStream.Position = 0;
        byte[] fileBytes = memoryStream.ToArray();

        ExportAccountResponse exportResult = new()
        {
            EncodedData = Convert.ToBase64String(fileBytes),
            ContentType = "application/json",
            FileName = "account-data.json"
        };

        return exportResult;
    }
}