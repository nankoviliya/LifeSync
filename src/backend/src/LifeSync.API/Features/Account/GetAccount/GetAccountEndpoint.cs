using FastEndpoints;
using LifeSync.API.Features.Account.GetAccount.Models;
using LifeSync.API.Features.Account.GetAccount.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.Account.GetAccount;

public sealed class GetAccountEndpoint : EndpointWithoutRequest<GetAccountResponse>
{
    private readonly IGetAccountService _getAccountService;

    public GetAccountEndpoint(IGetAccountService getAccountService) =>
        _getAccountService = getAccountService;

    public override void Configure()
    {
        Get("api/account");

        Summary(s =>
        {
            s.Summary = "Retrieves user profile data";
            s.Description = "Gets the profile information of the currently authenticated user. Returns user details if found.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<GetAccountResponse> result =
            await _getAccountService.GetUserAccountDataAsync(userId, ct);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await Send.ErrorsAsync(400, ct);
        }
        else
        {
            await Send.OkAsync(result.Data, ct);
        }
    }
}
