using FluentAssertions;
using LifeSync.API.Models.Languages;
using LifeSync.Common.Required;
using LifeSync.Tests.Unit.TestHelpers;

namespace LifeSync.Tests.Unit.Models.Languages;

public class LanguageTests
{
    [Fact]
    public void From_WithValidData_ShouldCreateLanguage()
    {
        var name = "English".ToRequiredString();
        var code = "en".ToRequiredString();

        var language = Language.From(name, code);

        language.Name.Should().Be("English");
        language.Code.Should().Be("en");
    }

    [Fact]
    public void From_WithUppercaseCode_ShouldNormalizeToLowercase()
    {
        var code = "EN".ToRequiredString();

        var language = Language.From("English".ToRequiredString(), code);

        language.Code.Should().Be("en");
    }

    [Fact]
    public void From_WithRegionCode_ShouldNormalizeProperly()
    {
        var code = "en-us".ToRequiredString();

        var language = Language.From("English (US)".ToRequiredString(), code);

        language.Code.Should().Be("en-US");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("eng")]
    [InlineData("en-US")]
    [InlineData("zh-CN")]
    public void From_WithValidCodeFormats_ShouldSucceed(string code)
    {
        var language = Language.From("Test Language".ToRequiredString(), code.ToRequiredString());

        language.Code.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("e")]
    [InlineData("engl")]
    [InlineData("en-")]
    [InlineData("en-U")]
    [InlineData("en-USA")]
    [InlineData("123")]
    public void From_WithInvalidCodeFormats_ShouldThrowArgumentException(string code)
    {
        Action act = () => Language.From("Test Language".ToRequiredString(), code.ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Language code must be a valid ISO 639 code*");
    }

    [Fact]
    public void From_WithNameTooShort_ShouldThrowArgumentException()
    {
        Action act = () => Language.From("A".ToRequiredString(), "en".ToRequiredString());

        act.Should().Throw<ArgumentException>()
            .WithMessage("Language name must be at least 2 characters.*");
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var language = CreateTestLanguage();

        language.UpdateName("British English".ToRequiredString());

        language.Name.Should().Be("British English");
    }

    [Fact]
    public void UpdateCode_WithValidCode_ShouldUpdateCode()
    {
        var language = CreateTestLanguage();

        language.UpdateCode("en-GB".ToRequiredString());

        language.Code.Should().Be("en-GB");
    }

    [Theory]
    [InlineData("en", true)]
    [InlineData("EN", true)]
    [InlineData("En", true)]
    [InlineData("fr", false)]
    public void IsCode_WithVariousCodes_ShouldMatchCorrectly(string codeToCheck, bool expectedResult)
    {
        var language = CreateTestLanguage();

        var result = language.IsCode(codeToCheck);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void GetBaseLanguageCode_WithTwoLetterCode_ShouldReturnSameCode()
    {
        var language = CreateTestLanguage("English", "en");

        var baseCode = language.GetBaseLanguageCode();

        baseCode.Should().Be("en");
    }

    [Fact]
    public void GetBaseLanguageCode_WithRegionCode_ShouldReturnBaseOnly()
    {
        var language = CreateTestLanguage("English (US)", "en-US");

        var baseCode = language.GetBaseLanguageCode();

        baseCode.Should().Be("en");
    }

    [Fact]
    public void HasRegion_WithRegionCode_ShouldReturnTrue()
    {
        var language = CreateTestLanguage("English (US)", "en-US");

        var hasRegion = language.HasRegion();

        hasRegion.Should().BeTrue();
    }

    [Fact]
    public void HasRegion_WithoutRegionCode_ShouldReturnFalse()
    {
        var language = CreateTestLanguage("English", "en");

        var hasRegion = language.HasRegion();

        hasRegion.Should().BeFalse();
    }

    [Fact]
    public void GetRegionCode_WithRegionCode_ShouldReturnRegion()
    {
        var language = CreateTestLanguage("English (US)", "en-US");

        var regionCode = language.GetRegionCode();

        regionCode.Should().Be("US");
    }

    [Fact]
    public void GetRegionCode_WithoutRegionCode_ShouldReturnNull()
    {
        var language = CreateTestLanguage("English", "en");

        var regionCode = language.GetRegionCode();

        regionCode.Should().BeNull();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var language = CreateTestLanguage("English", "en");

        var result = language.ToString();

        result.Should().Be("English (en)");
    }

    [Fact]
    public void Equality_WithSameId_ShouldBeEqual()
    {
        var (language1, language2) = EntityTestHelper.CreateTwoWithSameId(() =>
            Language.From("English".ToRequiredString(), "en".ToRequiredString()));

        language1.Should().Be(language2);
        (language1 == language2).Should().BeTrue();
        language1.GetHashCode().Should().Be(language2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentId_ShouldNotBeEqual()
    {
        var (language1, language2) = EntityTestHelper.CreateTwoWithDifferentIds(() =>
            Language.From("English".ToRequiredString(), "en".ToRequiredString()));

        language1.Should().NotBe(language2);
        (language1 == language2).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithSameIdButDifferentData_ShouldBeEqual()
    {
        var language1 = Language.From("English".ToRequiredString(), "en".ToRequiredString());
        var language2 = Language.From("French".ToRequiredString(), "fr".ToRequiredString());

        var sharedId = Guid.NewGuid();
        EntityTestHelper.SetId(language1, sharedId);
        EntityTestHelper.SetId(language2, sharedId);

        language1.Should().Be(language2);
    }

    private Language CreateTestLanguage(string name = "English", string code = "en")
    {
        return Language.From(name.ToRequiredString(), code.ToRequiredString());
    }
}
