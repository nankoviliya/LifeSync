using FastEndpoints;
using LifeSync.API.Features.Finances.Expenses.Models;
using LifeSync.API.Features.Finances.Expenses.Services;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using System.Security.Claims;

namespace LifeSync.API.Features.Finances.Expenses;

public sealed class AddExpenseEndpoint : Endpoint<AddExpenseRequest, AddExpenseResponse>
{
    private readonly IExpenseService _expenseService;

    public AddExpenseEndpoint(IExpenseService expenseService) =>
        _expenseService = expenseService;

    public override void Configure()
    {
        Post("api/finances/expenses");

        Summary(s =>
        {
            s.Summary = "Add an expense transaction";
            s.Description = "Creates a new expense transaction for the authenticated user.";
            s.Responses[200] = "Success";
            s.Responses[400] = "Bad Request";
            s.Responses[401] = "Unauthorized";
        });
    }

    public override async Task HandleAsync(AddExpenseRequest request, CancellationToken ct)
    {
        RequiredString userId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToRequiredString();

        DataResult<Guid> result = await _expenseService.AddExpenseAsync(userId, request, ct);

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
            await SendOkAsync(new AddExpenseResponse { TransactionId = result.Data }, ct);
        }
    }
}
