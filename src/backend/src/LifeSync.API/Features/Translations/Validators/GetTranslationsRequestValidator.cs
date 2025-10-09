using FluentValidation;
using LifeSync.API.Features.Translations.Models;

namespace LifeSync.API.Features.Translations.Validators;

public class GetTranslationsRequestValidator : AbstractValidator<GetTranslationsRequest>
{
    public GetTranslationsRequestValidator()
    {
        RuleFor(x => x.LanguageCode)
            .NotEmpty()
            .WithMessage("Language code is required.")
            .MaximumLength(10)
            .WithMessage("Language code must not exceed 10 characters.")
            .Matches(@"^[a-zA-Z]{2,3}(-[a-zA-Z]{2,4})?$")
            .WithMessage("Language code must be in valid format (e.g., 'en', 'en-US', 'bg').");
    }
}
