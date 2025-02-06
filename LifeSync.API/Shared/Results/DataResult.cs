namespace LifeSync.API.Shared.Results
{
    public class DataResult<T>
    {
        public bool IsSuccess { get; }

        public T Data
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("No data available for a failed result.");
                }

                return _data!;
            }
        }

        private readonly T? _data;

        public IReadOnlyCollection<string> Errors { get; }

        private DataResult(bool isSuccess, T? data, IEnumerable<string>? errors)
        {
            IsSuccess = isSuccess;
            _data = data;
            Errors = errors?.ToList().AsReadOnly() ?? Array.Empty<string>().AsReadOnly();
        }

        public static DataResult<T> Success(T data)
        {
            return new DataResult<T>(true, data, null);
        }

        public static DataResult<T> Failure(params string[] errors)
        {
            return new DataResult<T>(false, default, errors);
        }
    }
}
