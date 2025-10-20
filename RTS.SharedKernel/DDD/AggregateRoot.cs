using CSharpFunctionalExtensions;
using MediatR;

namespace RTS.SharedKernel.DDD
{
    public abstract class AggregateRoot : Entity<int>
    {
        public List<INotification> DomainEvents { get; private set; }
        protected AggregateRoot()
        {
            DomainEvents = new List<INotification>();
        }

        protected virtual void AddDomainEvent(INotification newEvent)
        {
            lock (DomainEvents)
                DomainEvents.Add(newEvent);
        }

        public virtual void ClearDomainEvents()
        {
            DomainEvents.Clear();
        }
    }
}