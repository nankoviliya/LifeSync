using FastEndpoints;
using LifeSync.API.Features.Finances.Incomes.Models;
using LifeSync.API.Features.Finances.Incomes.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace LifeSync.API.Features.Finances.Incomes;

public sealed class AddIncomeEndpoint : Endpoint<AddIncomeRequest, AddIncomeResponse>
{
    private readonly IIncomeService _incomeService;

    public AddIncomeEndpoint(IIncomeService incomeService) =>
        _incomeService = incomeService;

    public override void Configure()
    {
        Post("api/finances/transactions/income");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);

        Summary(s =>
        {
            s.Summary = "Add an income transaction";
            s.Description = "Creates a new income transaction for the authenticated user.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(AddIncomeRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<Guid> result = await _incomeService.AddIncomeAsync(userId, request, ct);

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
            await Send.OkAsync(new AddIncomeResponse { TransactionId = result.Data }, ct);
        }
    }
}