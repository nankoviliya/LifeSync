using LifeSync.API.Features.Users.Models;
using LifeSync.API.Features.Users.ResultMessages;
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
            return BadRequest(UsersResultMessages.UserIsNotAuthenticated);
        }

        var result = await usersService.GetUserProfileData(userId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPut("profile", Name = nameof(GetUserProfileData))]
    public async Task<IActionResult> ModifyUserProfileData([FromBody] ModifyUserProfileDataDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(UsersResultMessages.UserIsNotAuthenticated);
        }

        var result = await usersService.ModifyUserProfileData(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
}