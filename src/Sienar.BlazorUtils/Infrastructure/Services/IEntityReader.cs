using System;
using System.Threading.Tasks;
using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.Services;

public interface IEntityReader<TEntity>
{
	/// <summary>
	/// Gets an entity by primary key
	/// </summary>
	/// <param name="id">The primary key of the entity to retrieve</param>
	/// <param name="filter">A <see cref="Filter"/> to specify included results</param>
	/// <returns>the requested entity</returns>
	Task<TEntity?> Get(
		Guid id,
		Filter? filter = null);

	/// <summary>
	/// Gets a list of all entities in the backend
	/// </summary>
	/// <param name="filter">A <see cref="Filter"/> to specify included results</param>
	/// <returns>a list of all entities in the database</returns>
	Task<PagedDto<TEntity>> Get(Filter? filter = null);
}