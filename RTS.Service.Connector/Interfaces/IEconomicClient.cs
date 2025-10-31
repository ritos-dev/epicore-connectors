using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<ApiResult<string>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<ApiResult<string>> CreateInvoiceDraftAsync(string orderJson, string orderNumber, CancellationToken cancellationToken = default);
    }
}