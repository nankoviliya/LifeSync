using LifeSync.API.Features.Account.UpdateAccount.Models;
using LifeSync.Common.Required;
using LifeSync.Common.Results;

namespace LifeSync.API.Features.Account.UpdateAccount.Services;

public interface IUpdateAccountService
{
    Task<MessageResult> ModifyUserAccountDataAsync(
        RequiredString userId,
        UpdateAccountRequest data,
        CancellationToken cancellationToken);
}
