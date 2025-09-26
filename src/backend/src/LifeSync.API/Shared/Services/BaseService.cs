using LifeSync.Common.Results;

namespace LifeSync.API.Shared.Services;

public abstract class BaseService
{
    protected DataResult<T> Success<T>(T data) => DataResult<T>.Success(data);

    protected MessageResult SuccessMessage(string message = "") => MessageResult.Success(message);

    protected DataResult<T> Failure<T>(params string[] errors) => DataResult<T>.Failure(errors);

    protected MessageResult FailureMessage(params string[] errors) => MessageResult.Failure(errors);
}