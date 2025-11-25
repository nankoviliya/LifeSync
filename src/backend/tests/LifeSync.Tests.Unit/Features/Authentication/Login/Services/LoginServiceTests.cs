using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Login.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;

namespace LifeSync.Tests.Unit.Features.Authentication.Login.Services;

public class LoginServiceTests
{
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly LoginService _sut;

    public LoginServiceTests()
    {
        _userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null);

        ISecretsManager secretsManager = Substitute.For<ISecretsManager>();
        JwtSecrets jwtSecrets = JwtSecrets.Create(
            "super_secret_key_1234567890123456",
            "TestIssuer",
            "TestAudience",
            30);
        secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        _jwtTokenGenerator = new JwtTokenGenerator(secretsManager, new JwtSecurityTokenHandler());

        _sut = new LoginService(_jwtTokenGenerator, _userManager);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        User user = User.From(
            "test@example.com".ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(1000, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            Guid.NewGuid().ToRequiredStruct()
        );

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult(user));
        _userManager.CheckPasswordAsync(user, request.Password).Returns(Task.FromResult(true));

        DataResult<TokenResponse> result = await _sut.LoginAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().NotBeNullOrEmpty();
        result.Data.Expiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        LoginRequest request = new()
        {
            Email = "nonexistent@example.com",
            Password = "SomePassword123!"
        };

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult<User>(null));

        DataResult<TokenResponse> result = await _sut.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        User user = User.From(
            "test@example.com".ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(1000, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            Guid.NewGuid().ToRequiredStruct()
        );

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult(user));
        _userManager.CheckPasswordAsync(user, request.Password).Returns(Task.FromResult(false));

        DataResult<TokenResponse> result = await _sut.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_ShouldCallUserManagerFindByEmailAsync_WithCorrectEmail()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult<User>(null));

        await _sut.LoginAsync(request);

        await _userManager.Received(1).FindByEmailAsync(request.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldCallCheckPasswordAsync_WhenUserExists()
    {
        LoginRequest request = new()
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        User user = User.From(
            "test@example.com".ToRequiredString(),
            "test@example.com".ToRequiredString(),
            "John".ToRequiredString(),
            "Doe".ToRequiredString(),
            new Money(1000, "USD").ToRequiredReference(),
            "USD".ToRequiredString(),
            Guid.NewGuid().ToRequiredStruct()
        );

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult(user));
        _userManager.CheckPasswordAsync(user, request.Password).Returns(Task.FromResult(false));

        await _sut.LoginAsync(request);

        await _userManager.Received(1).CheckPasswordAsync(user, request.Password);
    }
}
