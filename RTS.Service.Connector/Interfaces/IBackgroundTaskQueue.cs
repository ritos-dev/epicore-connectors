namespace RTS.Service.Connector.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        ValueTask EnqueueAsync(string orderNumber, CancellationToken token);
        Task<string> DequeueAsync(CancellationToken token);
    }
}
