using LifeSync.API.Features.AccountImport.Models;

namespace LifeSync.API.Features.AccountImport.DataReaders;

public interface IAccountDataReader
{
    AccountImportFileFormat Format { get; }

    Task<ImportAccountData?> ReadAsync(IFormFile file, CancellationToken cancellationToken);
}