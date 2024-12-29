#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Data;
using Sienar.Processors;

namespace Sienar.Identity.Processors;

/// <exclude />
public class LockoutReasonFilterProcessor : IEntityFrameworkFilterProcessor<LockoutReason>
{
	public IQueryable<LockoutReason> Search(IQueryable<LockoutReason> dataset, Filter filter)
		=> string.IsNullOrEmpty(filter.SearchTerm)
			? dataset
			: dataset.Where(r => r.NormalizedReason.Contains(filter.SearchTerm.ToUpperInvariant()));

	public Expression<Func<LockoutReason, object>> GetSortPredicate(string? sortName) => r => r.Reason;
}