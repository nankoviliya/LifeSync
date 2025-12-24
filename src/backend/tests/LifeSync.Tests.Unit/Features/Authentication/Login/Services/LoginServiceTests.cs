using FluentAssertions;
using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Login.Services;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.Languages;
using LifeSync.API.Persistence;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.API.Shared;
using LifeSync.Common.Required;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;

namespace LifeSync.Tests.Unit.Features.Authentication.Login.Services;

public class LoginServiceTests
{
    private readonly string _testUserId;

    private readonly DbConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ISecretsManager _secretsManager;

    private readonly LoginService _sut;

    public LoginServiceTests()
    {
        _userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null);
        _secretsManager = Substitute.For<ISecretsManager>();

        JwtSecrets jwtSecrets = JwtSecrets.Create(
            "super_secret_key_1234567890123456",
            "TestIssuer",
            "TestAudience",
            30);
        _secretsManager.GetJwtSecretsAsync().Returns(Task.FromResult(jwtSecrets));

        _jwtTokenGenerator = new JwtTokenGenerator(_secretsManager, new JwtSecurityTokenHandler());

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        using ApplicationDbContext context = new(_contextOptions, _secretsManager);
        if (context.Database.EnsureCreated())
        {
            Language language = Language.From("English".ToRequiredString(), "en".ToRequiredString());

            context.Add(language);

            User user = User.From(
                "user123@gmail.com".ToRequiredString(),
                "user123@gmail.com".ToRequiredString(),
                "F".ToRequiredString(),
                "L".ToRequiredString(),
                new Money(200, "BGN").ToRequiredReference(),
                "BGN".ToRequiredString(),
                language.Id.ToRequiredStruct()
            );

            context.Add(user);

            _testUserId = user.Id.ToRequiredString();

            context.SaveChanges();
        }

        _sut = new LoginService(_userManager, _jwtTokenGenerator,
            new ApplicationDbContext(_contextOptions, _secretsManager));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        LoginRequest request = new() { Email = "test@example.com", Password = "ValidPassword123!" };

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

        DataResult<LoginResponse> result = await _sut.LoginAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.AccessToken.Should().NotBeNullOrEmpty();
        result.Data.AccessTokenExpiry.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        LoginRequest request = new() { Email = "nonexistent@example.com", Password = "SomePassword123!" };

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult<User>(null));

        DataResult<LoginResponse> result = await _sut.LoginAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        LoginRequest request = new() { Email = "test@example.com", Password = "WrongPassword123!" };

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

        DataResult<LoginResponse> result = await _sut.LoginAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_ShouldCallUserManagerFindByEmailAsync_WithCorrectEmail()
    {
        LoginRequest request = new() { Email = "test@example.com", Password = "ValidPassword123!" };

        _userManager.FindByEmailAsync(request.Email).Returns(Task.FromResult<User>(null));

        await _sut.LoginAsync(request, CancellationToken.None);

        await _userManager.Received(1).FindByEmailAsync(request.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldCallCheckPasswordAsync_WhenUserExists()
    {
        LoginRequest request = new() { Email = "test@example.com", Password = "ValidPassword123!" };

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

        await _sut.LoginAsync(request, CancellationToken.None);

        await _userManager.Received(1).CheckPasswordAsync(user, request.Password);
    }
}