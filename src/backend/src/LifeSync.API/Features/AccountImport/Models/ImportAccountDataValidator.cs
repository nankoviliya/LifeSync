using FluentValidation;
using LifeSync.API.Features.AccountImport.Models;

namespace LifeSync.API.Features.AccountImport.Validators;

public class ImportAccountDataValidator : AbstractValidator<ImportAccountData>
{
    public ImportAccountDataValidator()
    {
        RuleFor(x => x.ProfileData)
            .NotNull()
            .SetValidator(new ImportAccountProfileValidator());

        RuleFor(x => x.ExpenseTransactions)
            .NotNull();

        RuleForEach(x => x.ExpenseTransactions)
            .SetValidator(new ImportAccountExpenseTransactionValidator());

        RuleFor(x => x.IncomeTransactions)
            .NotNull();

        RuleForEach(x => x.IncomeTransactions)
            .SetValidator(new ImportAccountIncomeTransactionValidator());
    }
}

public class ImportAccountProfileValidator : AbstractValidator<ImportAccountProfile>
{
    public ImportAccountProfileValidator()
    {
        RuleFor(x => x.BalanceAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.BalanceCurrency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.LanguageCode)
            .NotEmpty()
            .Length(2);
    }
}

public class ImportAccountExpenseTransactionValidator : AbstractValidator<ImportAccountExpenseTransaction>
{
    private const int MaxDescriptionLength = 500;
    private const int MinDescriptionLength = 1;

    public ImportAccountExpenseTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(MinDescriptionLength)
            .MaximumLength(MaxDescriptionLength);

        RuleFor(x => x.ExpenseType)
            .IsInEnum();

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow);
    }
}

public class ImportAccountIncomeTransactionValidator : AbstractValidator<ImportAccountIncomeTransaction>
{
    private const int MaxDescriptionLength = 500;
    private const int MinDescriptionLength = 1;

    public ImportAccountIncomeTransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(MinDescriptionLength)
            .MaximumLength(MaxDescriptionLength);

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow);
    }
}