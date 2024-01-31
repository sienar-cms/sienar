using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Infrastructure.Entities;
using Sienar.Infrastructure.Processors;

namespace Sienar.Identity.Processors;

public class LockoutReasonFilterProcessor : IFilterProcessor<LockoutReason>
{
	/// <inheritdoc />
	public IQueryable<LockoutReason> Search(IQueryable<LockoutReason> dataset, Filter filter)
		=> string.IsNullOrEmpty(filter.SearchTerm)
			? dataset
			: dataset.Where(r => r.Reason.Contains(filter.SearchTerm));

	/// <inheritdoc />
	public Expression<Func<LockoutReason, object>> GetSortPredicate(string? sortName) => r => r.Reason;
}