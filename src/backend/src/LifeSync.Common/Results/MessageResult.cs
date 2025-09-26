namespace LifeSync.Common.Results;

public class MessageResult
{
    public bool IsSuccess { get; }

    public string Message { get; }

    public IReadOnlyCollection<string> Errors { get; }

    private MessageResult(bool isSuccess, string message, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Message = message;
        Errors = errors?.ToList().AsReadOnly() ?? Array.Empty<string>().AsReadOnly();
    }

    public static MessageResult Success(string message = "") => new(true, message, null);

    public static MessageResult Failure(params string[] errors) => new(false, string.Empty, errors);
}