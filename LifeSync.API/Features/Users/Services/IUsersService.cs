using LifeSync.API.Features.Users.Models;
using LifeSync.API.Shared.Results;

namespace LifeSync.API.Features.Users.Services;

public interface IUsersService
{
    Task<DataResult<GetUserProfileDataDto>> GetUserProfileData(string userId, CancellationToken cancellationToken);

    Task<MessageResult> ModifyUserProfileData(string userId, ModifyUserProfileDataDto data, CancellationToken cancellationToken);
}