namespace LifeSync.API.Features.AccountImport.Importers;

public interface IAccountImporter
{
    AccountImportFileFormat Format { get; }

    Task<ImportAccountData?> ImportAsync(IFormFile file, CancellationToken cancellationToken);
}
