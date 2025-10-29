using RTS.Service.Connector.Application.Contracts;

namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<ApiResult<string>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default);
        Task<ApiResult<string>> CreateInvoiceDraftAsync(string orderJson, CancellationToken cancellationToken = default);
    }
}