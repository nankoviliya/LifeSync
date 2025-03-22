using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData.ConcreteExporterFactory.ConcreteExporters;

public interface IAccountDataFileConcreteExporter
{
    /// <summary>
    /// The file format that this exporter supports (e.g., "json", "csv").
    /// </summary>
    ExportAccountFileFormat SupportedFormat { get; }

    Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(ExportAccountDataDto accountData);
}

