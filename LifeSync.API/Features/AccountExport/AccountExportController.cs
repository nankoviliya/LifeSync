using LifeSync.API.Features.Account.ResultMessages;
using LifeSync.API.Features.AccountExport.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LifeSync.API.Features.AccountExport;

[ApiController]
[Authorize]
[Route("api/accountExport")]
public class AccountExportController : ControllerBase
{
    private readonly IAccountExportService _accountExportService;

    public AccountExportController(IAccountExportService accountExportService)
    {
        _accountExportService = accountExportService;
    }

    [HttpPost("export")]
    [EndpointSummary("Exports account data in a desired format")]
    [EndpointDescription("Gets the profile information of the currently authenticated user. Returns data in specified file format.")]
    [Produces("application/json", "application/xml", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportUserAccountData([FromBody] ExportAccountRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(AccountResultMessages.UserIsNotAuthenticated);
        }

        var result = await _accountExportService.ExportAccountData(userId, request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var fileResult = result.Data;

        return File(fileResult.Data, fileResult.ContentType, fileResult.FileName);
    }
}
