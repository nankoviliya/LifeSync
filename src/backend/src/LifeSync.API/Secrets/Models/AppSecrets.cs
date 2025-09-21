using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets.Models;

public class AppSecrets
{
    [JsonPropertyName("DB_USER")]
    public string DbUser { get; set; }

    [JsonPropertyName("DB_PASSWD")]
    public string DbPasswd { get; set; }

    [JsonPropertyName("DB_HOST")]
    public string DbHost { get; set; }

    [JsonPropertyName("DB_PORT")]
    public int DbPort { get; set; }

    [JsonPropertyName("DB_NAME")]
    public string DbName { get; set; }
    
    [JsonPropertyName("JWT_SECRET_KEY")]
    public string JwtSecretKey { get; set; }

    [JsonPropertyName("JWT_ISSUER")]
    public string JwtIssuer { get; set; }

    [JsonPropertyName("JWT_AUDIENCE")]
    public string JwtAudience { get; set; }

    [JsonPropertyName("JWT_EXPIRY_MINUTES")]
    public int JwtExpiryMinutes { get; set; }

    [JsonIgnore]
    public DbConnectionSecrets Database =>
        DbConnectionSecrets.Create(DbUser, DbPasswd, DbHost, DbPort, DbName);

    [JsonIgnore]
    public JwtSecrets JWT => JwtSecrets.Create(JwtSecretKey, JwtIssuer, JwtAudience, JwtExpiryMinutes);
}