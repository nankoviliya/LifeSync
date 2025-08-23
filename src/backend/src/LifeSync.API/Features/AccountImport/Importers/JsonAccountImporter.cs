using System.Text.Json;

namespace LifeSync.API.Features.AccountImport.Importers;

public class JsonAccountImporter : IAccountImporter
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public AccountImportFileFormat Format => AccountImportFileFormat.Json;

    public async Task<ImportAccountData?> ImportAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var data = await JsonSerializer.DeserializeAsync<ImportAccountData>(
            stream, CachedJsonSerializerOptions, cancellationToken);

        return data;
    }
}
