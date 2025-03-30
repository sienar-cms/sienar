#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class SienarRoleFilterProcessor : IFilterProcessor<SienarRole>
{
	public IQueryable<SienarRole> Search(IQueryable<SienarRole> dataset, Filter filter) => dataset;

	public Expression<Func<SienarRole, object>> GetSortPredicate(string? sortName) => r => r.Name;
}