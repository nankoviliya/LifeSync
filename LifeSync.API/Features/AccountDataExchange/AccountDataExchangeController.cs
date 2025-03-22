using LifeSync.API.Features.Account.ResultMessages;
using LifeSync.API.Features.AccountDataExchange.Models;
using LifeSync.API.Features.AccountDataExchange.Services;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.AccountDataExchange
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountDataExchangeController : ControllerBase
    {
        private readonly IAccountDataExchangeService _accountDataExchangeService;

        public AccountDataExchangeController(IAccountDataExchangeService accountDataExchangeService)
        {
            _accountDataExchangeService = accountDataExchangeService;
        }

        [HttpGet("export-data")]
        [EndpointSummary("Exports account data in a desired format")]
        [EndpointDescription("Gets the profile information of the currently authenticated user. Returns data in specified file format.")]
        [Produces("application/json", "application/xml", "text/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ExportUserAccountData([FromQuery] ExportAccountFileFormat fileFormat)
        {
            var userId = "2197615C-2B99-41C9-BF75-DA95BC68D6D4";
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(AccountResultMessages.UserIsNotAuthenticated);
            }

            var result = await _accountDataExchangeService.ExportAccountData(userId, fileFormat);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            var fileResult = result.Data;

            return File(fileResult.Data, fileResult.ContentType, fileResult.FileName);
        }
    }
}
