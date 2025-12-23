using System.Security.Cryptography;

namespace LifeSync.API.Features.Authentication.Helpers;

public interface ICsrfTokenGenerator
{
    string Generate();
}

public sealed class CsrfTokenGenerator : ICsrfTokenGenerator
{
    public string Generate()
    {
        byte[] randomBytes = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        return Convert.ToBase64String(randomBytes);
    }
}
