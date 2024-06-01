using System;
using System.Linq;
using System.Linq.Expressions;
using Sevita.Tools.Data;
using Sienar.Infrastructure.Data;
using Sienar.Infrastructure.Processors;

namespace Sevita.Tools.Processors;

public class ObjectiveProcessor : IEntityFrameworkFilterProcessor<Objective>
{
	/// <inheritdoc />
	public IQueryable<Objective> Search(IQueryable<Objective> dataset, Filter filter) => throw new NotImplementedException();

	/// <inheritdoc />
	public Expression<Func<Objective, object>> GetSortPredicate(string? sortName) => throw new NotImplementedException();
}