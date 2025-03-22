using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.AccountDataExchange.Services.ExportData
{
    public interface IAccountDataFileExporter
    {
        Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(ExportAccountFileFormat fileFormat, ExportAccountDataDto accountData);
    }
}