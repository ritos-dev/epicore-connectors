using Ardalis.Specification;
using CSharpFunctionalExtensions;

namespace RTS.SharedKernel.Interfaces
{
    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : Entity<int>
    {
    }
}