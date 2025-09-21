using System.Text.Json.Serialization;

namespace LifeSync.API.Secrets.Models;

public class DbConnectionSecrets
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string DbInstanceIdentifier { get; set; }
    
    private DbConnectionSecrets(
        string username, 
        string password, 
        string host, 
        int port, 
        string dbInstanceIdentifier)
    {
        Username = username;
        Password = password;
        Host = host;
        Port = port;
        DbInstanceIdentifier = dbInstanceIdentifier;
    }

    public static DbConnectionSecrets Create(
        string username, 
        string password, 
        string host, 
        int port, 
        string dbInstanceIdentifier)
    {
        ValidateDbCredentials(username, password, host, port, dbInstanceIdentifier);
        
        return new DbConnectionSecrets(username, password, host, port, dbInstanceIdentifier);
    }
    
    private static void ValidateDbCredentials(string user, string passwd, string host, int port, string name)
    {
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("Database user cannot be empty", nameof(user));
        
        if (string.IsNullOrWhiteSpace(passwd))
            throw new ArgumentException("Database password cannot be empty", nameof(passwd));
        
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Database host cannot be empty", nameof(host));
        
        if (port <= 0 || port > 65535)
            throw new ArgumentException("Database port must be between 1 and 65535", nameof(port));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Database name cannot be empty", nameof(name));
    }
}