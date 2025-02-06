using LifeSync.API.Shared.Results;

namespace LifeSync.API.Shared.Services
{
    public abstract class BaseService
    {
        protected DataResult<T> Success<T>(T data)
        {
            return DataResult<T>.Success(data);
        }

        protected MessageResult SuccessMessage(string message = "")
        {
            return MessageResult.Success(message);
        }

        protected DataResult<T> Failure<T>(params string[] errors)
        {
            return DataResult<T>.Failure(errors);
        }

        protected MessageResult FailureMessage(params string[] errors)
        {
            return MessageResult.Failure(errors);
        }
    }
}
