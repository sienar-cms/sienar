using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class SienarRoleFilterProcessor : IFilterProcessor<SienarRole>
{
	/// <inheritdoc />
	public IQueryable<SienarRole> Search(IQueryable<SienarRole> dataset, Filter filter) => dataset;

	/// <inheritdoc />
	public Expression<Func<SienarRole, object>> GetSortPredicate(string? sortName) => r => r.Name;
}