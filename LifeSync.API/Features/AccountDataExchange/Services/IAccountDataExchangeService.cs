using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.AccountDataExchange.Services
{
    public interface IAccountDataExchangeService
    {
        Task<DataResult<ExportAccountFileResultDto>> ExportAccountData(string userId, ExportAccountFileFormat fileFormat);
    }
}
