using LifeSync.API.Features.AccountExport.Models;

namespace LifeSync.API.Features.AccountExport.DataExporters;

public interface IAccountDataExporter
{
    ExportAccountFileFormat Format { get; }

    Task<ExportAccountResponse> Export(ExportAccountData accountData, CancellationToken cancellationToken);
}