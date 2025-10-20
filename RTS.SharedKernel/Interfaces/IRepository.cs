using Ardalis.Specification;
using RTS.SharedKernel.DDD;

namespace RTS.SharedKernel.Interfaces
{
    public interface IRepository<T> : IRepositoryBase<T> where T : AggregateRoot
    {
    }
}