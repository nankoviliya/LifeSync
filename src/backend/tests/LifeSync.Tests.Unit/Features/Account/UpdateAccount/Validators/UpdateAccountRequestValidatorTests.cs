using FluentAssertions;
using FluentValidation.Results;
using LifeSync.API.Features.Account.UpdateAccount.Models;

namespace LifeSync.UnitTests.Features.Account.UpdateAccount.Validators;

public class UpdateAccountRequestValidatorTests
{
    private readonly UpdateAccountRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = "John", LastName = "Doe", LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsEmpty()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = string.Empty, LastName = "Doe", LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "First name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsEmpty()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = "John", LastName = string.Empty, LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Last name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLanguageIdIsEmpty()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = "John", LastName = "Doe", LanguageId = string.Empty
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Language ID is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameExceedsMaxLength()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = new string('a', 101), LastName = "Doe", LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "First name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameExceedsMaxLength()
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = "John", LastName = new string('a', 101), LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Last name must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("John")]
    [InlineData("Mary Jane")]
    [InlineData("O'Brien")]
    [InlineData("José")]
    public void Validate_ShouldPass_WhenFirstNameIsValid(string firstName)
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = firstName, LastName = "Doe", LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("Doe")]
    [InlineData("Van Der Berg")]
    [InlineData("O'Connor")]
    [InlineData("García")]
    public void Validate_ShouldPass_WhenLastNameIsValid(string lastName)
    {
        UpdateAccountRequest request = new UpdateAccountRequest
        {
            FirstName = "John", LastName = lastName, LanguageId = Guid.NewGuid().ToString()
        };

        ValidationResult? result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}