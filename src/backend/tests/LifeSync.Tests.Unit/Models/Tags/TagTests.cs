using FluentAssertions;
using LifeSync.API.Models.Tags;
using LifeSync.Common.Required;

namespace LifeSync.Tests.Unit.Models.Tags;

public class TagTests
{
    [Fact]
    public void From_WithValidName_ShouldCreateTag()
    {
        Tag tag = Tag.From("dentist".ToRequiredString(), "user-1".ToRequiredString());

        tag.Name.Should().Be("dentist");
        tag.UserId.Should().Be("user-1");
    }

    [Fact]
    public void From_WithUppercaseName_ShouldLowercaseSilently()
    {
        Tag tag = Tag.From("Dentist".ToRequiredString(), "user-1".ToRequiredString());

        tag.Name.Should().Be("dentist");
    }

    [Fact]
    public void From_WithSurroundingWhitespace_ShouldTrim()
    {
        Tag tag = Tag.From("  dentist  ".ToRequiredString(), "user-1".ToRequiredString());

        tag.Name.Should().Be("dentist");
    }

    [Fact]
    public void From_WithMixedCaseAndHyphens_ShouldNormalize()
    {
        Tag tag = Tag.From("Health-Care".ToRequiredString(), "user-1".ToRequiredString());

        tag.Name.Should().Be("health-care");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithEmptyName_ShouldThrow(string name)
    {
        Action act = () => Tag.From(name.ToRequiredString(), "user-1".ToRequiredString());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithNameLongerThan50Chars_ShouldThrow()
    {
        string longName = new string('a', 51);
        Action act = () => Tag.From(longName.ToRequiredString(), "user-1".ToRequiredString());
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("foo bar")]      // space
    [InlineData("foo.bar")]      // punctuation
    [InlineData("foo_bar")]      // underscore
    [InlineData("foo!")]         // symbol
    [InlineData("café")]         // non-ASCII
    public void From_WithInvalidCharacters_ShouldThrow(string name)
    {
        Action act = () => Tag.From(name.ToRequiredString(), "user-1".ToRequiredString());
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("-foo")]
    [InlineData("foo-")]
    [InlineData("foo--bar")]
    public void From_WithBadHyphenPlacement_ShouldThrow(string name)
    {
        Action act = () => Tag.From(name.ToRequiredString(), "user-1".ToRequiredString());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void From_WithEmptyUserId_ShouldThrow()
    {
        Action act = () => Tag.From("dentist".ToRequiredString(), "".ToRequiredString());
        act.Should().Throw<ArgumentException>();
    }
}
