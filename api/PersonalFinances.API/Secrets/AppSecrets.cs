using System.Text.Json.Serialization;

namespace PersonalFinances.API.Secrets;

public class AppSecrets
{
    [JsonPropertyName("Database")]
    public DbConnectionSecrets Database { get; set; }
    
    [JsonPropertyName("JWT")]
    public JwtSecrets JWT { get; set; }
}