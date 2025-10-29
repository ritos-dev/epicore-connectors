namespace RTS.Service.Connector.Application.Contracts
{
    public sealed record ApiResult<T>
    (
        bool IsSuccess,
        T? Data,
        string? ErrorMessage = null
    )
    {
        public static ApiResult<T> Success(T data) => new(true, data);
        public static ApiResult<T> Failure(string message) => new(false, default, message);
    }
}
