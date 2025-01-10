using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PersonalFinances.API.Features.Authentication.Models;
using PersonalFinances.API.Models;
using PersonalFinances.API.Models.ApplicationUser;
using PersonalFinances.API.Secrets;

namespace PersonalFinances.API.Features.Authentication.Helpers;

public class JwtTokenGenerator
{
    private readonly JwtSecrets _jwtSecrets;

    public JwtTokenGenerator(JwtSecrets jwtSecrets)
    {
        _jwtSecrets = jwtSecrets;
    }
    
    public TokenResponse GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecrets.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSecrets.Issuer,
            audience: _jwtSecrets.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSecrets.ExpiryMinutes),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return new TokenResponse
        {
            Token = tokenString,
            Expiry = token.ValidTo
        };
    }
}