using Ardalis.Specification;
using CSharpFunctionalExtensions;

namespace RTS.SharedKernel.Extensions
{
    public static class SpecificationBuilderExtensions
    {
        public static ISpecificationBuilder<T> OrderedPaginate<T>(this ISpecificationBuilder<T> @this, int page, int pageSize)
            where T : Entity<int>
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            @this.Specification.Query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize);

            return @this;
        }
    }
}
