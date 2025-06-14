namespace LifeSync.API.Secrets.Exceptions;

public class SecretsRetrievalException : Exception
{
    public SecretsRetrievalException(string message)
        : base(message)
    {
    }

    public SecretsRetrievalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
