using System.Security.Claims;
using LifeSync.API.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Users;

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService usersService;

    public UsersController(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    [HttpGet("profile", Name = nameof(GetUserProfileData))]
    public async Task<IActionResult> GetUserProfileData()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result = await usersService.GetUserProfileData(userId);

        return Ok(result);
    }
}