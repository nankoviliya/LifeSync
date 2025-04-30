namespace LifeSync.API.Features.AccountExport.Exporters;

public interface IAccountExporter
{
    ExportAccountFileFormat Format { get; }

    Task<ExportAccountResponse> Export(ExportAccountData accountData, CancellationToken cancellationToken);
}
