using MediatR;

namespace RTS.SharedKernel.Interfaces
{
    public interface IHandleEvent<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
    {
    }
}