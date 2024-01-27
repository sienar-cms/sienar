using System;
using System.Linq;
using System.Linq.Expressions;

namespace Sienar.Infrastructure.Entities;

public interface IFilterProcessor<TEntity>
{
	/// <summary>
	/// Applies included entities to the <see cref="IQueryable"/>
	/// </summary>
	/// <param name="dataset">The <see cref="IQueryable{TEntity}"/> containing the current query</param>
	/// <param name="filter">The <see cref="Filter"/></param>
	/// <returns>the updated <see cref="IQueryable{TEntity}"/></returns>
	IQueryable<TEntity> ProcessIncludes(IQueryable<TEntity> dataset, Filter filter) => dataset;

	/// <summary>
	/// Performs a search of the <see cref="IQueryable{TEntity}"/>
	/// </summary>
	/// <param name="dataset">The <see cref="IQueryable{TEntity}"/> containing the current query</param>
	/// <param name="filter">The <see cref="Filter"/></param>
	/// <returns>the updated <see cref="IQueryable{TEntity}"/></returns>
	IQueryable<TEntity> Search(IQueryable<TEntity> dataset, Filter filter);

	/// <summary>
	/// Creates a predicate for use in sorting by columns
	/// </summary>
	/// <param name="sortName">The name of the column to sort by</param>
	/// <returns>the sort predicate</returns>
	Expression<Func<TEntity, object>> GetSortPredicate(string? sortName);
}