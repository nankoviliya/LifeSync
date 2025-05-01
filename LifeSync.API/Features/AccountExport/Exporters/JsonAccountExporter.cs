using System.Text.Json;

namespace LifeSync.API.Features.AccountExport.Exporters;

public class JsonAccountExporter : IAccountExporter
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public ExportAccountFileFormat Format => ExportAccountFileFormat.Json;

    public async Task<ExportAccountResponse> Export(ExportAccountData accountData, CancellationToken cancellationToken)
    {
        await using var memoryStream = new MemoryStream();

        await JsonSerializer.SerializeAsync(memoryStream, accountData, CachedJsonSerializerOptions, cancellationToken);

        memoryStream.Position = 0;
        byte[] fileBytes = memoryStream.ToArray();

        var exportResult = new ExportAccountResponse
        {
            Data = fileBytes,
            ContentType = "application/json",
            FileName = "account-data.json"
        };

        return exportResult;
    }
}
