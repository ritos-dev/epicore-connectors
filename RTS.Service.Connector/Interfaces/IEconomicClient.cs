using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.BackgroundWorker;

namespace RTS.Service.Connector.Interfaces
{
    public interface IEconomicClient
    {
        Task<ApiResult<EconomicInvoiceDraftDto>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken);
        Task<ApiResult<EconomicInvoiceDraftDto>> CreateInvoiceDraftAsync(EconomicInvoiceDraftDto draft, string orderNumber, string crmNumber, CancellationToken cancellationToken);
    }
}