namespace RTS.Service.Connector.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        void Enqueue(string orderNumber);
        Task<string> DequeueAsync(CancellationToken token);
    }
}
