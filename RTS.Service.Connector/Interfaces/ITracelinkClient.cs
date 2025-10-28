using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<TracelinkResults<OrderDto>> GetOrderAsync(string orderId,CancellationToken cancellationToken = default);
        Task<TracelinkResults<OrderDto>> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default);
    }
}
