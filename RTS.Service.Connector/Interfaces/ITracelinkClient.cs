using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<ApiResult<TracelinkOrderDto>> GetOrderAsync(string orderId,CancellationToken cancellationToken = default);
        Task<ApiResult<TracelinkOrderDto>> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default);
    }
}
