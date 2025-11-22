using LifeSync.API.Features.Authentication.Register.Models;
using System.Net.Http.Json;

namespace LifeSync.Tests.Integration.Infrastructure;

public static class DefaultUserAccount
{
    public static RegisterRequest RegisterUserRequest = new()
    {
        FirstName = "Test",
        LastName = "User",
        Email = "testUser@gmail.com",
        Password = "Test!Pa$$Wor1d",
        Balance = 1000,
        Currency = "BGN",
        LanguageId = Guid.Parse("A0B10002-1A2B-4C3D-9E10-000000000002")
    };

    public static async Task InsertDefaultUserAsync(this HttpClient httpClient, RegisterRequest request) =>
        await httpClient.PostAsJsonAsync($"/api/auth/register", request);
}