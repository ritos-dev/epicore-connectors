using Azure;
using RTS.SharedKernel.Commons;

namespace RTS.SharedKernel.Interfaces
{
    public interface IEventPublisher
    {
        Task<Response> PublishEventAsync(string subject, string eventType, EventPublisherDto data);
    }
}