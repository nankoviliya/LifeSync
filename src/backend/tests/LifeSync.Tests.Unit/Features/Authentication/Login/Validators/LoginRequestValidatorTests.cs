using FluentAssertions;
using LifeSync.API.Features.Authentication.Login.Models;

namespace LifeSync.UnitTests.Features.Authentication.Login.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("test.user@example.co.uk")]
    [InlineData("admin@subdomain.example.com")]
    [InlineData("contact+info@test.org")]
    public void Validate_ShouldPass_WhenEmailIsValid(string email)
    {
        LoginRequest request = new()
        {
            Email = email,
            Password = "SomePassword"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsEmpty()
    {
        LoginRequest request = new()
        {
            Email = string.Empty,
            Password = "ValidPassword123!"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Email is required.");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@@example.com")]
    public void Validate_ShouldFail_WhenEmailIsInvalid(string email)
    {
        LoginRequest request = new()
        {
            Email = email,
            Password = "ValidPassword123!"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "A valid email is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsEmpty()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = string.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenBothEmailAndPasswordAreEmpty()
    {
        LoginRequest request = new()
        {
            Email = string.Empty,
            Password = string.Empty
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(e => e.ErrorMessage == "Email is required.");
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password is required.");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("password")]
    [InlineData("VeryLongPasswordThatIsStillValid123!@#")]
    public void Validate_ShouldPass_WhenPasswordIsNotEmpty(string password)
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = password
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenEmailIsWhitespace()
    {
        LoginRequest request = new()
        {
            Email = "   ",
            Password = "ValidPassword"
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordIsWhitespace()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "   "
        };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Password is required.");
    }
}
