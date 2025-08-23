using FluentValidation;
using LifeSync.API.Features.Finances.Models;

namespace LifeSync.API.Features.Finances.Validators;

public class AddIncomeDtoValidator : AbstractValidator<AddIncomeDto>
{
    public AddIncomeDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Income amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency code must be exactly 3 characters.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required.")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Income date cannot be in the future.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must be 500 characters or less.");
    }
}
