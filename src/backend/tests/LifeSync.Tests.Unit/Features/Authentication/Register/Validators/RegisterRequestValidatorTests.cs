using FluentAssertions;
using LifeSync.API.Features.Authentication.Register.Models;

namespace LifeSync.Tests.Unit.Features.Authentication.Register.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "SecurePassword123!",
            Balance = 1000m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = string.Empty,
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "First name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFirstNameExceedsMaxLength()
    {
        RegisterRequest request = new()
        {
            FirstName = new string('A', 101),
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "First name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenFirstNameIsExactly100Characters()
    {
        RegisterRequest request = new()
        {
            FirstName = new string('A', 100),
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = string.Empty,
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Last name is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLastNameExceedsMaxLength()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = new string('B', 101),
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Last name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenLastNameIsExactly100Characters()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = new string('B', 100),
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = string.Empty,
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Email is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public void Validate_ShouldFail_WhenEmailIsInvalid(string email)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "A valid email is required.");
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("test.user@example.co.uk")]
    [InlineData("admin@subdomain.example.com")]
    public void Validate_ShouldPass_WhenEmailIsValid(string email)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = email,
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = string.Empty,
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password is required.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenBalanceIsZero()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 0m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenBalanceIsNegative()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = -100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Balance must be non-negative.");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(100)]
    [InlineData(999999.99)]
    public void Validate_ShouldPass_WhenBalanceIsPositive(decimal balance)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = balance,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenCurrencyIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = string.Empty,
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Currency is required.");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("U")]
    [InlineData("USDD")]
    [InlineData("US D")]
    public void Validate_ShouldFail_WhenCurrencyIsNot3Characters(string currency)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = currency,
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Currency code must be exactly 3 characters.");
    }

    [Theory]
    [InlineData("XXX")]
    [InlineData("ABC")]
    [InlineData("ZZZ")]
    public void Validate_ShouldFail_WhenCurrencyIsNotSupported(string currency)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = currency,
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("is not supported"));
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("BGN")]
    [InlineData("UAH")]
    public void Validate_ShouldPass_WhenCurrencyIsSupported(string currency)
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = currency,
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenLanguageIdIsEmpty()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Language selection is required.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenLanguageIdIsValid()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "test@example.com",
            Password = "Password123!",
            Balance = 100m,
            Currency = "USD",
            LanguageId = Guid.NewGuid()
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
    {
        RegisterRequest request = new()
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = "invalid-email",
            Password = string.Empty,
            Balance = -50m,
            Currency = "XX",
            LanguageId = Guid.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(7);
    }
}
