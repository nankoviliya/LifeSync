using LifeSync.API.Features.AccountExport.Models;
using System.Text.Json;

namespace LifeSync.API.Features.AccountExport.Exporters;

public class JsonAccountExporter : IAccountExporter
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public ExportAccountFileFormat Format => ExportAccountFileFormat.Json;

    public async Task<ExportAccountResult> Export(ExportAccountDto accountData)
    {
        await using var memoryStream = new MemoryStream();

        await JsonSerializer.SerializeAsync(memoryStream, accountData, CachedJsonSerializerOptions);

        memoryStream.Position = 0;
        byte[] fileBytes = memoryStream.ToArray();

        var exportResult = new ExportAccountResult
        {
            Data = fileBytes,
            ContentType = "application/json",
            FileName = "account-data.json"
        };

        return exportResult;
    }
}
