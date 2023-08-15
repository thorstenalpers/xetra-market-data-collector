namespace MarketData.Domain.Repositories;

using Ardalis.Specification;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IEntity<int>
{
}
