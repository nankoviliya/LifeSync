using FluentAssertions;
using LifeSync.API.Features.Translations.Models;
using LifeSync.API.Features.Translations.Validators;

namespace LifeSync.UnitTests.Features.Translations.Validators;

public class GetTranslationsRequestValidatorTests
{
    private readonly GetTranslationsRequestValidator _validator = new();

    [Theory]
    [InlineData("en")]
    [InlineData("bg")]
    [InlineData("ru")]
    [InlineData("uk")]
    [InlineData("en-US")]
    [InlineData("en-GB")]
    [InlineData("zh-CN")]
    public void Validate_ShouldPass_WhenLanguageCodeIsValid(string languageCode)
    {
        var request = new GetTranslationsRequest { LanguageCode = languageCode };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenLanguageCodeIsEmpty()
    {
        var request = new GetTranslationsRequest { LanguageCode = string.Empty };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Language code is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLanguageCodeIsNull()
    {
        var request = new GetTranslationsRequest { LanguageCode = null! };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Language code is required.");
    }

    [Theory]
    [InlineData("e")]           // Too short
    [InlineData("english")]     // Too long
    [InlineData("en_US")]       // Wrong separator
    [InlineData("123")]         // Numbers
    [InlineData("en-")]         // Incomplete
    [InlineData("-US")]         // Missing language part
    [InlineData("en-U")]        // Country code too short
    [InlineData("en-USAAA")]    // Country code too long
    public void Validate_ShouldFail_WhenLanguageCodeIsInvalid(string languageCode)
    {
        var request = new GetTranslationsRequest { LanguageCode = languageCode };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("valid format"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenLanguageCodeExceedsMaxLength()
    {
        var request = new GetTranslationsRequest { LanguageCode = "en-US-extra" };

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Language code must not exceed 10 characters.");
    }
}
