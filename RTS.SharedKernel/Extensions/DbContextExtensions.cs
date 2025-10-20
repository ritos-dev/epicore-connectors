using MediatR;
using Microsoft.EntityFrameworkCore;
using RTS.SharedKernel.DDD;

namespace RTS.SharedKernel.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task CommitAsync(this DbContext _dataContext, IMediator _mediator)
        {
            var domainEntities = _dataContext.ChangeTracker.Entries<AggregateRoot>().Where(x => x.Entity.DomainEvents.Count > 0);
            var tasks = new List<Task>();
            foreach (var entity in domainEntities)
            {
                foreach (var e in entity.Entity.DomainEvents)
                {
                    tasks.Add(_mediator.Publish(e));
                }
            }

            await _dataContext.SaveChangesAsync();

            try
            {
                await Task.WhenAll(tasks);
                foreach (var entity in domainEntities)
                {
                    entity.Entity.ClearDomainEvents();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
