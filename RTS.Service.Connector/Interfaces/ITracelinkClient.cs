using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.Application.DTOs;

namespace RTS.Service.Connector.Interfaces
{
    public interface ITracelinkClient
    {
        Task<TracelinkResults<ExternalOrderDto>> GetOrderAsync(
            string orderId, 
            CancellationToken cancellationToken = default);
    }
}
