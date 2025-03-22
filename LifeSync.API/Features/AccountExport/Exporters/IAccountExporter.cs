using LifeSync.API.Features.AccountExport.Models;

namespace LifeSync.API.Features.AccountExport.Exporters;

public interface IAccountExporter
{
    ExportAccountFileFormat Format { get; }

    Task<ExportAccountResult> Export(ExportAccountDto accountData);
}
