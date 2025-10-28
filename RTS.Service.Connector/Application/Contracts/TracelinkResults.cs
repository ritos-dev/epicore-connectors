namespace RTS.Service.Connector.Application.Contracts
{
    public sealed record TracelinkResults<T>
    (
        bool IsSuccess,
        T? Data,
        string? ErrorMessage = null
    );
}
