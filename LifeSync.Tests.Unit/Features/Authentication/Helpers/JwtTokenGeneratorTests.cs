using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Secrets;

namespace LifeSync.UnitTests.Features.Authentication.Helpers;

public class JwtTokenGeneratorTests
{   
    [Fact]
    public void GenerateJwtToken_ShouldReturnTokenResponse_WhenUserIsValid()
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

        var generator = new JwtTokenGenerator(jwtSecrets);

        var tokenResponse = generator.GenerateJwtToken(user);

        tokenResponse.Token.Should().NotBeNullOrEmpty();
        tokenResponse.Expiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GenerateJwtToken_ShouldIncludeCorrectClaims_WhenUserIsValid()
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

        var generator = new JwtTokenGenerator(jwtSecrets);

        var tokenResponse = generator.GenerateJwtToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateJwtToken_ShouldThrowArgumentException_WhenSecretKeyIsInvalid()
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

        var generator = new JwtTokenGenerator(jwtSecrets);

        Action action = () => generator.GenerateJwtToken(user);

        action.Should().Throw<ArgumentException>().WithMessage("*key*");
    }

    [Fact]
    public void GenerateJwtToken_ShouldThrowArgumentNullException_WhenUserIsNull()
    {
        var jwtSecrets = new JwtSecrets
        {
            SecretKey = "super_secret_key_1234567890123456",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 30
        };

        var generator = new JwtTokenGenerator(jwtSecrets);

        Action action = () => generator.GenerateJwtToken(null);

        action.Should().Throw<NullReferenceException>();
    }
}