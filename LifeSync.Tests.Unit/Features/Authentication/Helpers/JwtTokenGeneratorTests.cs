using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;

namespace LifeSync.UnitTests.Features.Authentication.Helpers;

public class JwtTokenGeneratorTests
{
    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldReturnTokenResponse_WhenUserIsValid()
    {
        var jwtSecrets = new JwtSecrets
        {
            SecretKey = "super_secret_key_1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 30
        };

        var user = new User
        {
            Id = "user123",
            Email = "test@example.com"
        };

        var secretsManager = Substitute.For<ISecretsManager>();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        var generator = new JwtTokenGenerator(secretsManager);

        var tokenResponse = await generator.GenerateJwtTokenAsync(user);

        tokenResponse.Token.Should().NotBeNullOrEmpty();
        tokenResponse.Expiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldIncludeCorrectClaims_WhenUserIsValid()
    {
        var jwtSecrets = new JwtSecrets
        {
            SecretKey = "super_secret_key_1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 30
        };

        var user = new User
        {
            Id = "user123",
            Email = "test@example.com"
        };

        var secretsManager = Substitute.For<ISecretsManager>();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        var generator = new JwtTokenGenerator(secretsManager);

        var tokenResponse = await generator.GenerateJwtTokenAsync(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public async Task GenerateJwtTokenAsync_ShouldThrowArgumentException_WhenSecretKeyIsInvalid()
    {
        var jwtSecrets = new JwtSecrets
        {
            SecretKey = "",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 30
        };

        var user = new User
        {
            Id = "user123",
            Email = "test@example.com"
        };

        var secretsManager = Substitute.For<ISecretsManager>();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        var generator = new JwtTokenGenerator(secretsManager);

        Func<Task> act = async () => await generator.GenerateJwtTokenAsync(user);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*key*");
    }

    [Fact]
    public async Task GenerateJwtToken_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        var jwtSecrets = new JwtSecrets
        {
            SecretKey = "super_secret_key_1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 30
        };

        var secretsManager = Substitute.For<ISecretsManager>();

        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        var generator = new JwtTokenGenerator(secretsManager);

        Func<Task> act = async () => await generator.GenerateJwtTokenAsync(null);

        await act.Should().ThrowAsync<NullReferenceException>();
    }
}