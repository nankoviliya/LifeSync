using LifeSync.API.Features.Users.Models;
using LifeSync.API.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User is not authenticated.");
        }

        var result = await usersService.GetUserProfileData(userId);

        return Ok(result);
    }

    [HttpPut("profile", Name = nameof(GetUserProfileData))]
    public async Task<IActionResult> ModifyUserProfileData([FromBody] ModifyUserProfileDataDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User is not authenticated.");
        }

        await usersService.ModifyUserProfileData(userId, request);

        return Ok();
    }
}