using LifeSync.API.Features.Account.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Account.Services.Contracts;

public interface IAccountService
{
    Task<DataResult<GetUserAccountDataDto>> GetUserAccountData(string userId);

    Task<MessageResult> ModifyUserAccountData(string userId, ModifyUserAccountDataDto data);
}