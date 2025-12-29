using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;

namespace LifeSync.Tests.Unit.Features.Authentication.Helpers;

public class JwtTokenGeneratorTests
{
    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnTokenResponse_WhenUserIsValid()
    {
        JwtSecrets jwtSecrets = JwtSecrets.Create(
            "super_secret_key_1234567890123456",
            "TestIssuer",
            "TestAudience",
            30);

        User user = User.From(
            "user123@gmail.com".ToRequiredString(),
            "user123@gmail.com".ToRequiredString(),
            "F".ToRequiredString(),
            "L".ToRequiredString(),
            new Money(200, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            Guid.Parse("BE9E8EEA-E3A4-475E-B364-08218FB3CC6C").ToRequiredStruct()
        );

        ISecretsManager? secretsManager = Substitute.For<ISecretsManager>();
        JwtSecurityTokenHandler securityTokenHandler = new();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        JwtTokenGenerator generator = new(secretsManager, securityTokenHandler);

        TokenResponse tokenResponse = await generator.GenerateAccessTokenAsync(user, DeviceType.Web);

        tokenResponse.Token.Should().NotBeNullOrEmpty();
        tokenResponse.Expiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldIncludeCorrectClaims_WhenUserIsValid()
    {
        JwtSecrets jwtSecrets = JwtSecrets.Create(
            "super_secret_key_1234567890123456",
            "TestIssuer",
            "TestAudience",
            30);

        User user = User.From(
            "user123@gmail.com".ToRequiredString(),
            "user123@gmail.com".ToRequiredString(),
            "F".ToRequiredString(),
            "L".ToRequiredString(),
            new Money(200, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            Guid.Parse("BE9E8EEA-E3A4-475E-B364-08218FB3CC6C").ToRequiredStruct()
        );

        ISecretsManager? secretsManager = Substitute.For<ISecretsManager>();
        JwtSecurityTokenHandler securityTokenHandler = new();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        JwtTokenGenerator generator = new(secretsManager, securityTokenHandler);

        TokenResponse tokenResponse = await generator.GenerateAccessTokenAsync(user, DeviceType.Web);
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken? jwtToken = handler.ReadJwtToken(tokenResponse.Token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldThrowInvalidOperationException_WhenSecretsAreNull()
    {
        User user = User.From(
            "user123@gmail.com".ToRequiredString(),
            "user123@gmail.com".ToRequiredString(),
            "F".ToRequiredString(),
            "L".ToRequiredString(),
            new Money(200, "BGN").ToRequiredReference(),
            "BGN".ToRequiredString(),
            Guid.Parse("BE9E8EEA-E3A4-475E-B364-08218FB3CC6C").ToRequiredStruct()
        );

        ISecretsManager? secretsManager = Substitute.For<ISecretsManager>();
        JwtSecurityTokenHandler securityTokenHandler = new();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult<JwtSecrets>(null));

        JwtTokenGenerator generator = new(secretsManager, securityTokenHandler);

        Func<Task> act = async () => await generator.GenerateAccessTokenAsync(user, DeviceType.Web);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateJwtToken_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        JwtSecrets jwtSecrets = JwtSecrets.Create(
            "super_secret_key_1234567890123456",
            "TestIssuer",
            "TestAudience",
            30);

        ISecretsManager? secretsManager = Substitute.For<ISecretsManager>();
        JwtSecurityTokenHandler securityTokenHandler = new();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        JwtTokenGenerator generator = new(secretsManager, securityTokenHandler);

        Func<Task> act = async () => await generator.GenerateAccessTokenAsync(null, DeviceType.Web);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}