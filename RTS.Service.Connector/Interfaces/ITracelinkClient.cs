using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<ApiResult<OrderDto>> GetOrderAsync(string orderId,CancellationToken cancellationToken = default);
        Task<ApiResult<OrderDto>> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default);
    }
}
