namespace LifeSync.API.Shared.Results
{
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

        public static MessageResult Success(string message = "")
        {
            return new MessageResult(true, message, null);
        }

        public static MessageResult Failure(params string[] errors)
        {
            return new MessageResult(false, string.Empty, errors);
        }
    }
}
