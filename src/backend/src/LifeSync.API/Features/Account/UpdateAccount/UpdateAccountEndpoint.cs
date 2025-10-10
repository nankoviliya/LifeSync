using FastEndpoints;
using LifeSync.API.Features.Account.UpdateAccount.Models;
using LifeSync.API.Features.Account.UpdateAccount.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.Account.UpdateAccount;

public sealed class UpdateAccountEndpoint : Endpoint<UpdateAccountRequest, string>
{
    private readonly IUpdateAccountService _updateAccountService;

    public UpdateAccountEndpoint(IUpdateAccountService updateAccountService) =>
        _updateAccountService = updateAccountService;

    public override void Configure()
    {
        Put("api/account");

        Summary(s =>
        {
            s.Summary = "Modifies user profile data";
            s.Description = "Updates the profile information of the authenticated user using the provided details.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(UpdateAccountRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        MessageResult result =
            await _updateAccountService.ModifyUserAccountDataAsync(userId, request, ct);

        if (!result.IsSuccess)
        {
            foreach (string error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(400, ct);
        }
        else
        {
            await SendOkAsync(result.Message, ct);
        }
    }
}
