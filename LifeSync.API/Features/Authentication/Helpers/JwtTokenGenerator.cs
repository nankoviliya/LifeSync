using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Secrets.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LifeSync.API.Features.Authentication.Helpers;

public class JwtTokenGenerator
{
    private readonly ISecretsManager secretsManager;

    public JwtTokenGenerator(ISecretsManager secretsManager)
    {
        this.secretsManager = secretsManager;
    }

    public async Task<TokenResponse> GenerateJwtTokenAsync(User user)
    {
        var jwtSecrets = await secretsManager.GetJwtSecretsAsync();

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecrets.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSecrets.Issuer,
            audience: jwtSecrets.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSecrets.ExpiryMinutes),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse
        {
            Token = tokenString,
            Expiry = token.ValidTo
        };
    }
}