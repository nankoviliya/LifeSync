using FluentValidation;
using LifeSync.API.Shared;

namespace LifeSync.API.Features.Authentication.Register.Models;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Balance)
            .GreaterThanOrEqualTo(0).WithMessage("Balance must be non-negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency code must be exactly 3 characters.")
            .Must(CurrencyRegistry.IsSupported)
            .WithMessage(x =>
                $"Currency '{x.Currency}' is not supported. Supported currencies: {CurrencyRegistry.GetSupportedCodesString()}.");

        RuleFor(x => x.LanguageId)
            .NotEmpty().WithMessage("Language selection is required.");
    }
}