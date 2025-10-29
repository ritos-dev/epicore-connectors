namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<string?> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<bool> CreateInvoiceDraftAsync(string orderJson, CancellationToken cancellationToken = default);
    }
}