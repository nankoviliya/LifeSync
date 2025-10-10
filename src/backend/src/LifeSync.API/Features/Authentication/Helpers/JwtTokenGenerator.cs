using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Secrets.Contracts;
using LifeSync.API.Secrets.Models;
using LifeSync.Common.Required;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LifeSync.API.Features.Authentication.Helpers;

public class JwtTokenGenerator
{
    private readonly ISecretsManager _secretsManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenGenerator(
        ISecretsManager secretsManager,
        JwtSecurityTokenHandler tokenHandler)
    {
        _secretsManager = secretsManager;
        _tokenHandler = tokenHandler;
    }

    public async Task<TokenResponse> GenerateJwtTokenAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        JwtSecrets? jwtSecrets = await _secretsManager.GetJwtSecretsAsync();

        if (jwtSecrets is null)
        {
            throw new ArgumentNullException(nameof(jwtSecrets));
        }

        Claim[]? claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.ToRequiredString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        byte[]? keyBytes = Encoding.UTF8.GetBytes(jwtSecrets.SecretKey);
        SymmetricSecurityKey? key = new(keyBytes);
        SigningCredentials? signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor? tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSecrets.ExpiryMinutes),
            Issuer = jwtSecrets.Issuer,
            Audience = jwtSecrets.Audience,
            SigningCredentials = signingCredentials
        };

        SecurityToken? token = _tokenHandler.CreateToken(tokenDescriptor);
        string? tokenString = _tokenHandler.WriteToken(token);

        return new TokenResponse { Token = tokenString, Expiry = token.ValidTo };
    }
}