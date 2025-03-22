
using LifeSync.API.Features.AccountExport.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.AccountExport;

public interface IAccountExportService
{
    Task<DataResult<ExportAccountResult>> ExportAccountData(string userId, ExportAccountRequest request);
}
