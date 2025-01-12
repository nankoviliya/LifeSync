using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets;

public class JwtSecrets
{
    [JsonPropertyName("SecretKey")]
    public string SecretKey { get; set; }
    
    [JsonPropertyName("Issuer")]
    public string Issuer { get; set; }
    
    [JsonPropertyName("Audience")]
    public string Audience { get; set; }
    
    [JsonPropertyName("ExpiryMinutes")]
    public int ExpiryMinutes { get; set; }
}