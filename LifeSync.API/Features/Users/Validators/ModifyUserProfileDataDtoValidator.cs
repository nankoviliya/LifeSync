using FluentValidation;
using LifeSync.API.Features.Users.Models;

namespace LifeSync.API.Features.Users.Validators
{
    public class ModifyUserProfileDataDtoValidator : AbstractValidator<ModifyUserProfileDataDto>
    {
        public ModifyUserProfileDataDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(500).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(500).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.LanguageId)
                .NotEmpty().WithMessage("Language ID is required.");
        }
    }
}
