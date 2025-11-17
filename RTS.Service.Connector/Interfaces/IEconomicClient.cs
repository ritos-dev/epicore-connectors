using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<ApiResult<string>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken);
        Task<ApiResult<string>> CreateInvoiceDraftAsync(EconomicInvoiceDraft draft, string orderNumber, string crmNumber, CancellationToken cancellationToken);
    }
}