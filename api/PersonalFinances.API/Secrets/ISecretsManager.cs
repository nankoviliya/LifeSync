namespace PersonalFinances.API.Secrets;

public interface ISecretsManager
{
    Task<string> GetConnectionStringAsync();
}