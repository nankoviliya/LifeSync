using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Features.AccountDataExchange.Services.ExportData.ConcreteExporterFactory.ConcreteExporters;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData.Factory
{
    public class AccountDataFileExporterFactory : IAccountDataFileExporterFactory
    {
        private readonly IEnumerable<IAccountDataFileConcreteExporter> _exporters;

        public AccountDataFileExporterFactory(IEnumerable<IAccountDataFileConcreteExporter> exporters)
        {
            _exporters = exporters;
        }

        public IAccountDataFileConcreteExporter GetExporter(ExportAccountFileFormat fileFormat)
        {
            var exporter = _exporters.FirstOrDefault(e => e.SupportedFormat == fileFormat);

            if (exporter is null)
            {
                throw new NotSupportedException($"File format '{fileFormat}' is not supported.");
            }

            return exporter;
        }
    }
}
