using FluentAssertions;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Features.Authentication.Register.Models;
using LifeSync.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Features.Authentication;

public class LoginEndpointTests : IntegrationTestsBase
{
    public LoginEndpointTests(IntegrationTestsWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_ReturnsValidTokenResponse_WhenRequestIsSuccessful()
    {
        RegisterRequest registerUserRequest = DefaultUserAccount.RegisterUserRequest;

        LoginRequest loginRequest = new()
        {
            Email = registerUserRequest.Email, Password = registerUserRequest.Password
        };

        HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

        loginResponse.Should().NotBeNull();

        loginResponse.AccessToken.Should().NotBeNullOrEmpty();
        loginResponse.AccessTokenExpiry.Should().BeAfter(DateTime.UtcNow);

        // Verify token allows access to authenticated endpoints
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
        HttpResponseMessage
            authenticatedResponse = await HttpClient.GetAsync("/api/account"); // Should be 100% with auth

        authenticatedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}