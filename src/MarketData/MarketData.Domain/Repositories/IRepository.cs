﻿namespace MarketData.Domain.Repositories;

using Ardalis.Specification;

public interface IRepository<T> : IRepositoryBase<T> where T : class, IEntity<int>
{
}

