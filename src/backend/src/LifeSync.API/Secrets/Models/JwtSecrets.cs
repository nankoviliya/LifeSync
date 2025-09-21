using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets.Models;

public class JwtSecrets
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpiryMinutes { get; set; }
    
    private JwtSecrets(
        string secretKey, 
        string issuer, 
        string audience, 
        int expiryMinutes)
    {
        SecretKey = secretKey;
        Issuer = issuer;
        Audience = audience;
        ExpiryMinutes = expiryMinutes;
    }

    public static JwtSecrets Create(
        string secretKey, 
        string issuer, 
        string audience, 
        int expiryMinutes)
    {
        ValidateJwtConfiguration(secretKey, issuer, audience, expiryMinutes);
        
        return new JwtSecrets(secretKey, issuer, audience, expiryMinutes);
    }
    
    private static void ValidateJwtConfiguration(string secretKey, string issuer, string audience, int expiryMinutes)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new ArgumentException("JWT secret key cannot be empty", nameof(secretKey));
        
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException("JWT issuer cannot be empty", nameof(issuer));
        
        if (string.IsNullOrWhiteSpace(audience))
            throw new ArgumentException("JWT audience cannot be empty", nameof(audience));
        
        if (expiryMinutes <= 0)
            throw new ArgumentException("JWT expiry minutes must be positive", nameof(expiryMinutes));
    }
}