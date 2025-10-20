using MediatR;

namespace RTS.SharedKernel.Interfaces
{
    public interface IDomainEvent : INotification
    {
        DateTime DateOccurred { get; }
    }
}