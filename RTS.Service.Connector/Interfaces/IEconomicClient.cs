using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.BackgroundWorker;

namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<ApiResult<EconomicInvoiceDraft>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken);
        Task<ApiResult<EconomicInvoiceDraft>> CreateInvoiceDraftAsync(EconomicInvoiceDraft draft, string orderNumber, string crmNumber, CancellationToken cancellationToken);
    }
}