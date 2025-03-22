using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Features.AccountDataExchange.Services.ExportData.ConcreteExporterFactory.ConcreteExporters;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData.Factory
{
    public interface IAccountDataFileExporterFactory
    {
        IAccountDataFileConcreteExporter GetExporter(ExportAccountFileFormat fileFormat);
    }
}
