using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Features.AccountDataExchange.Services.ExportData.ConcreteExporterFactory.ConcreteExporters;
using LifeSync.API.Features.AccountDataExchange.Services.ExportData.Factory;
using LifeSync.API.Shared.Results;
using LifeSync.API.Shared.Services;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData
{
    public class AccountDataFileExporter : BaseService, IAccountDataFileExporter
    {
        private readonly IAccountDataFileExporterFactory _accountDataFileExporterFactory;

        public AccountDataFileExporter(IAccountDataFileExporterFactory accountDataFileExporterFactory)
        {
            _accountDataFileExporterFactory = accountDataFileExporterFactory;
        }

        public async Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(ExportAccountFileFormat fileFormat, ExportAccountDataDto accountData)
        {
            IAccountDataFileConcreteExporter exporter;

            try
            {
                exporter = _accountDataFileExporterFactory.GetExporter(fileFormat);
            }
            catch (NotSupportedException ex)
            {
                return Failure<ExportAccountFileResultDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return Failure<ExportAccountFileResultDto>(ex.Message);
            }

            return await exporter.ExportAccountData(accountData);
        }
    }
}
