using FluentAssertions;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Authentication;

public class RegisterEndpointTests : IntegrationTestsBase
{
    public RegisterEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_CreatesNewUser_WhenRequestIsValid()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@gmail.com",
            Password = "Test!Pa$$Wor1d",
            Balance = 1000,
            Currency = "BGN",
            LanguageId = Guid.Parse("A0B10002-1A2B-4C3D-9E10-000000000002")
        };

        HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"/api/auth/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        User? user = DbContext.Users.FirstOrDefault(x => x.Email == request.Email);

        user.Should().NotBeNull();

        user.FirstName.Should().Be(request.FirstName);
        user.LastName.Should().Be(request.LastName);
        user.Email.Should().Be(request.Email);
        user.Balance.Amount.Should().Be(request.Balance);
        user.Balance.CurrencyCode.Should().Be(request.Currency);
        user.LanguageId.Should().Be(request.LanguageId);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUserWithSameEmailAlreadyExists()
    {
        RegisterRequest request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@gmail.com",
            Password = "Test!Pa$$Wor1d",
            Balance = 1000,
            Currency = "BGN",
            LanguageId = Guid.Parse("A0B10002-1A2B-4C3D-9E10-000000000002")
        };

        HttpResponseMessage firstUserCreateResponse = await HttpClient.PostAsJsonAsync($"/api/auth/register", request);

        firstUserCreateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        HttpResponseMessage secondUserCreateResponse = await HttpClient.PostAsJsonAsync($"/api/auth/register", request);

        secondUserCreateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // TODO: deserialize and assess entire response model
        string content = await secondUserCreateResponse.Content.ReadAsStringAsync();
        content.Should().Contain($"Username '{request.Email}' is already taken.");
    }
}