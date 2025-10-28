namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<bool> OrderExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
    }
}