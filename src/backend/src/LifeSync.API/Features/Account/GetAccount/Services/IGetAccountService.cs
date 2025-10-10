using LifeSync.API.Features.Account.GetAccount.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Account.GetAccount.Services;

public interface IGetAccountService
{
    Task<DataResult<GetAccountResponse>> GetUserAccountDataAsync(
        RequiredString userId,
        CancellationToken cancellationToken);
}
