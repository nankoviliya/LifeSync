using LifeSync.API.Features.Account.Models;
using LifeSync.API.Features.Account.ResultMessages;
using LifeSync.API.Features.Account.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LifeSync.API.Features.Account;

[ApiController]
[Authorize]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [EndpointSummary("Retrieves user profile data")]
    [EndpointDescription("Gets the profile information of the currently authenticated user. Returns user details if found.")]
    [ProducesResponseType<GetUserAccountDataDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserAccountData(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(AccountResultMessages.UserIsNotAuthenticated);
        }

        var result = await _accountService.GetUserAccountData(userId, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Data);
    }

    [HttpPut]
    [EndpointSummary("Modifies user profile data")]
    [EndpointDescription("Updates the profile information of the authenticated user using the provided details.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ModifyUserAccountData(
        [FromBody] ModifyUserAccountDataDto request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(AccountResultMessages.UserIsNotAuthenticated);
        }

        var result = await _accountService.ModifyUserAccountData(userId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}