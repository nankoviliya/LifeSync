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
    [EndpointSummary("Retrieves user profile data")]
    [EndpointDescription("Gets the profile information of the currently authenticated user. Returns user details if found.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserProfileDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserProfileData(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(UsersResultMessages.UserIsNotAuthenticated);
        }

        var result = await usersService.GetUserProfileData(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPut("profile", Name = nameof(GetUserProfileData))]
    [EndpointSummary("Modifies user profile data")]
    [EndpointDescription("Updates the profile information of the authenticated user using the provided details.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ModifyUserProfileData(
        [FromBody] ModifyUserProfileDataDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(UsersResultMessages.UserIsNotAuthenticated);
        }

        var result = await usersService.ModifyUserProfileData(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok();
    }
}