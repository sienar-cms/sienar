using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class GoalProcessor : IEntityFrameworkFilterProcessor<Goal>
{
	/// <inheritdoc />
	public IQueryable<Goal> Search(IQueryable<Goal> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Goal, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}