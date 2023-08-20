namespace MarketData.Infrastructure.Repositories;

using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using MarketData.Application.Repositories;

public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IEntity<int>
{
    public EfRepository(MarketDataDbContext dbContext) : base(dbContext)
    {
    }
}