namespace MarketData.Application.Repositories;

using Ardalis.Specification;

public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IEntity<int>
{
}
