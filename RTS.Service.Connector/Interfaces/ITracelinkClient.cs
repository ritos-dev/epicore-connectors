using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<ApiResult<TracelinkOrderListDto>> GetOrderListAsync(string orderId, CancellationToken token);
        Task<ApiResult<TracelinkOrderDto>> GetOrderByIdAsync(string orderId, CancellationToken token);
        Task<ApiResult<TracelinkCustomerDto>> GetCustomerListAsync(string customerName, CancellationToken token);
        Task<ApiResult<TracelinkCRMDto>> GetCrmListAsync(string customerId, CancellationToken token);
        Task<ApiResult<List<TracelinkItemsDto>>> GetItemsFromCrmAsync (string crmId, CancellationToken token);
        Task<ApiResult<TracelinkItemsListDto>> GetItemListAsync(string objectId, CancellationToken token);
    }
}
