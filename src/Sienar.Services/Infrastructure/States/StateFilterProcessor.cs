using System;
using System.Linq;
using System.Linq.Expressions;
using Sienar.Infrastructure.Entities;

namespace Sienar.Infrastructure.States;

public class StateFilterProcessor : IFilterProcessor<State>
{
	/// <inheritdoc />
	public IQueryable<State> Search(IQueryable<State> results, Filter filter)
	{
		if (string.IsNullOrEmpty(filter.SearchTerm))
		{
			return results;
		}

		return results.Where(
			s => s.Name.ToLower()
					.Contains(filter.SearchTerm) ||
				s.Abbreviation.ToLower()
					.Contains(filter.SearchTerm));
	}

	/// <inheritdoc />
	public Expression<Func<State, object>> GetSortPredicate(string? sortName) => sortName switch
	{
		nameof(State.Abbreviation) => s => s.Abbreviation,
		_ => s => s.Name
	};
}