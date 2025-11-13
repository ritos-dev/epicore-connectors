using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<ApiResult<TracelinkOrderListDto>> GetOrderListAsync(string orderId, CancellationToken token);
        Task<ApiResult<TracelinkDto>> GetOrderByIdAsync(string orderId, CancellationToken token);
        Task<ApiResult<TracelinkCustomerDto>> GetCustomerListAsync(string customerName, CancellationToken token);
        Task<ApiResult<TracelinkCRMDto>> GetCrmListAsync(string customerId, CancellationToken token);
    }
}
