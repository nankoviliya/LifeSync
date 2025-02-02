using LifeSync.API.Features.Users.Models;

namespace LifeSync.API.Features.Users.Services;

public interface IUsersService
{
    Task<GetUserProfileDataDto?> GetUserProfileData(string userId);

    Task ModifyUserProfileData(string userId, ModifyUserProfileDataDto data);
}