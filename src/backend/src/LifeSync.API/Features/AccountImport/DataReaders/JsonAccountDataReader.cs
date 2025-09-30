using LifeSync.API.Features.AccountImport.Models;
using System.Text.Json;

namespace LifeSync.API.Features.AccountImport.DataReaders;

public class JsonAccountDataReader : IAccountDataReader
{
    private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new() { WriteIndented = true };

    public AccountImportFileFormat Format => AccountImportFileFormat.Json;

    public async Task<ImportAccountData?> ReadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        await using Stream stream = file.OpenReadStream();

        ImportAccountData? data = await JsonSerializer.DeserializeAsync<ImportAccountData>(
            stream, CachedJsonSerializerOptions, cancellationToken);

        return data;
    }
}